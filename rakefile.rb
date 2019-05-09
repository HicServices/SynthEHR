require 'albacore'
load 'rakeconfig.rb'
$MSBUILD15CMD = MSBUILD15CMD.gsub(/\\/,"/")

task :continuous, [:config] => [:assemblyinfo, :build, :tests]

task :release, [:config] => [:assemblyinfo, :deploy, :pack]

task :restorepackages do
    sh "nuget restore #{SOLUTION}"
end

msbuild :build, [:config] => :restorepackages do |msb, args|
	args.with_defaults(:config => :Debug)
	msb.command = $MSBUILD15CMD
    msb.properties = { :configuration => args.config }
    msb.targets = [ :Clean, :Build ]   
    msb.solution = SOLUTION
end

task :tests do 
	sh 'dotnet test --logger:"nunit;LogFilePath=test-result.xml"'
end

desc "Runs all tests"
nunit :test do |nunit|
	files = FileList["*Tests/**/*Tests.dll"].exclude(/obj\//)
	nunit.command = "packages/NUnit.ConsoleRunner.3.9.0/tools/nunit3-console.exe"
	nunit.assemblies files.to_a
	nunit.options "--workers=1 --inprocess --result=\"nunit-results.xml\";format=nunit2 --noheader --labels=After"
end

msbuild :deploy, [:config] => :restorepackages do |msb, args|
	args.with_defaults(:config => :Release)
	msb.command = $MSBUILD15CMD
    msb.targets [ :Clean, :Build ]
    msb.properties = { :configuration => args.config }
    msb.solution = SOLUTION
end

desc "Sets the version number from GIT"    
assemblyinfo :assemblyinfo do |asm|
	asm.input_file = "SharedAssemblyInfo.cs"
    asm.output_file = "SharedAssemblyInfo.cs"
    asminfoversion = File.read("SharedAssemblyInfo.cs")[/\d+\.\d+\.\d+(\.\d+)?/]
        
    major, minor, patch, build = asminfoversion.split(/\./)
   
    if PRERELEASE == "true"
        build = build.to_i + 1
        $SUFFIX = "-pre"
    elsif CANDIDATE == "true"
        build = build.to_i + 1
        $SUFFIX = "-rc"
    end

    $VERSION = "#{major}.#{minor}.#{patch}.#{build}"
    puts "version: #{$VERSION}#{$SUFFIX}"
    # DO NOT REMOVE! needed by build script!
    f = File.new('version', 'w')
    f.write "#{$VERSION}#{$SUFFIX}"
    f.close
    # ----
    asm.version = $VERSION
    asm.file_version = $VERSION
    asm.informational_version = "#{$VERSION}#{$SUFFIX}"
end

desc "Pushes the plugin packages into the specified folder"    
task :pack, [:config] do |t, args|
	args.with_defaults(:config => :Release)
    Dir.chdir('NuGet') do
        sh "nuget pack BadMedicine.nuspec -Properties Configuration=#{args.config} -IncludeReferencedProjects -Symbols -Version #{$VERSION}#{$SUFFIX}"
        sh "nuget push BadMedicine.#{$VERSION}#{$SUFFIX}.nupkg -Source https://api.nuget.org/v3/index.json -ApiKey #{NUGETKEY}"
    end
end
