using System;
using BadMedicine.Datasets;

namespace BadMedicine
{
    /// <summary>
    /// Args for the <see cref="IDataGenerator.RowsGenerated"/> event.  Describes how many rows have
    /// been generated and how long has gone by etc.
    /// </summary>
    public class RowsGeneratedEventArgs
    {

        /// <summary>
        /// The current number of rows written (may differ from the line numbers of the file generated
        /// if the records contain newlines etc).
        /// </summary>
        public int RowsWritten { get; private set; }

        /// <summary>
        /// The length of time elapsed since record writing began
        /// </summary>
        public TimeSpan ElapsedTime { get; private set; }

        /// <summary>
        /// False for all event invocations except the last.  When this property is true you know the
        /// report generator has finished
        /// </summary>
        public bool IsFinished { get; private set; }

        public RowsGeneratedEventArgs(int rowsWritten, TimeSpan elapsedTime, bool isFinished)
        {
            RowsWritten = rowsWritten;
            ElapsedTime = elapsedTime;
            IsFinished = isFinished;
        }
    }
}