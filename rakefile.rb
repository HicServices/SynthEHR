require "net/http"
require 'uri'
require 'json'

load 'rakeconfig.rb'
$MSBUILD15CMD = MSBUILD15CMD.gsub(/\\/,"/")

task :continuous, [:config] => [:assemblyinfo, :build, :tests]

task :release, [:config] => [:assemblyinfo, :deploy, :build_cli, :github, :pack]

task :restorepackages do
    sh "nuget restore #{SOLUTION}"
end

task :build, [:config] => :restorepackages do |t, args|
	args.with_defaults(:config => :Debug)
	sh "\"#{$MSBUILD15CMD}\" #{SOLUTION} \/t:Clean;Build \/p:Configuration=#{args.config}"
end

task :tests do 
	sh 'dotnet test --logger:"nunit;LogFilePath=test-result.xml"'
end

task :deploy, [:config] => :restorepackages do |t, args|
	args.with_defaults(:config => :Release)
	sh "\"#{$MSBUILD15CMD}\" #{SOLUTION} \/t:Clean;Build \/p:Configuration=#{args.config}"
end

desc "Sets the version number from SharedAssemblyInfo file"    
task :assemblyinfo do 
	asminfoversion = File.read("SharedAssemblyInfo.cs").match(/AssemblyInformationalVersion\("(\d+)\.(\d+)\.(\d+)(-.*)?"/)
    
	puts asminfoversion.inspect
	
    major = asminfoversion[1]
	minor = asminfoversion[2]
	patch = asminfoversion[3]
    suffix = asminfoversion[4]
	
	version = "#{major}.#{minor}.#{patch}"
    puts "version: #{version}#{suffix}"
    
	# DO NOT REMOVE! needed by build script!
    f = File.new('version', 'w')
    f.write "#{version}#{suffix}"
    f.close
    # ----
end

desc "Pushes the plugin packages into the specified folder"    
task :pack, [:config] do |t, args|
	args.with_defaults(:config => :Release)
    Dir.chdir('BadMedicine.Core') do
        sh "nuget pack BadMedicine.nuspec -Properties Configuration=#{args.config} -IncludeReferencedProjects -Symbols -Version #{$VERSION}#{$SUFFIX}"
        sh "nuget push HIC.BadMedicine.#{$VERSION}#{$SUFFIX}.nupkg -Source https://api.nuget.org/v3/index.json -ApiKey #{NUGETKEY}"
    end
end

task :build_cli => :restorepackages do
	Dir.chdir("BadMedicine/") do
        sh "dotnet publish -r win-x64 -c Release -o Publish"
		Dir.chdir("Publish/") do
			sh "#{SQUIRREL}/signtool.exe sign /a /s MY /n \"University of Dundee\" /fd sha256 /tr http://sha256timestamp.ws.symantec.com/sha256/timestamp /td sha256 /v *.dll"
			sh "#{SQUIRREL}/signtool.exe sign /a /s MY /n \"University of Dundee\" /fd sha256 /tr http://sha256timestamp.ws.symantec.com/sha256/timestamp /td sha256 /v *.exe"
		end
    end
	sh "powershell.exe -nologo -noprofile -command \"& { Add-Type -A 'System.IO.Compression.FileSystem'; [IO.Compression.ZipFile]::CreateFromDirectory('BadMedicine/Publish', 'BadMedicine/badmedicine-cli-win-x64.zip'); }\""
end

task :github do
	version = File.open('version') {|f| f.readline}
    puts "version: #{version}"
	branch = "master" # (ENV['BRANCH_SELECTOR'] || "origin/master").gsub(/origin\//, "")
	puts branch
	prerelease = false # branch.match(/master/) ? false : true	
	
	uri = URI.parse('https://api.github.com/repos/HicServices/BadMedicine/releases')
	body = { tag_name: "v#{version}", name: "BadMedicine v#{version}", body: ENV['MESSAGE'] || "Compiled binary (for win-x64) of BadMedicine v#{version}", target_commitish: branch, prerelease: prerelease }
    header = {'Content-Type' => 'application/json',
              'Authorization' => "token #{GITHUB}"}
	
	http = Net::HTTP.new(uri.host, uri.port)
	http.use_ssl = (uri.scheme == "https")
	request = Net::HTTP::Post.new(uri.request_uri, header)
	request.body = body.to_json

	# Send the request
	response = http.request(request)
    puts response.to_hash.inspect
    githubresponse = JSON.parse(response.body)
    puts githubresponse.inspect
    upload_url = githubresponse["upload_url"].gsub(/\{.*\}/, "")
    puts upload_url
    	
	Dir.chdir("BadMedicine") do
		upload_to_github(upload_url, "badmedicine-cli-win-x64.zip")
    end
end

def upload_to_github(upload_url, file_path)
    boundary = "AaB03x"
    uri = URI.parse(upload_url + "?name=" + file_path)
    
    header = {'Content-Type' => 'application/octet-stream',
              'Content-Length' => File.size(file_path).to_s,
              'Authorization' => "token #{GITHUB}"}

    http = Net::HTTP.new(uri.host, uri.port)
    http.use_ssl = (uri.scheme == "https")
    request = Net::HTTP::Post.new(uri.request_uri, header)

    file = File.open(file_path, "rb")
    request.body = file.read
    
    response = http.request(request)
    
    puts response.to_hash.inspect
    puts response.body

    file.close
end