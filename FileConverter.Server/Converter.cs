namespace FileConverter.Server
{
    public static class Converter
    {
        /// <summary>
        /// Convert a csv file to tsv
        /// </summary>
        /// <param name="inputFilePath"></param>
        /// <param name="outputFilePath"></param>
        /// <param name="direction"></param>
        /// <param name="noOfLines"></param>
        /// <exception cref="Exception"></exception>
        public static void ConvertToTSVFile(string inputFilePath, string outputFilePath, char? direction = null, int? noOfLines = null)
        {
            string headerRecord;
            Console.WriteLine("Loading csv file " + inputFilePath);

            if (File.Exists(inputFilePath))
            {
                // Reads all the lines of the file and converts to string collection
                IEnumerable<string>? listOfRecords = File.ReadAllLines(inputFilePath)?.ToList();

                if (listOfRecords?.Any() ?? false)
                {
                    // Taking into consideration that we have header in the file, we don't want that row to be considered for selection
                    // Removing Header Row and adding it to headerRow
                    headerRecord = listOfRecords.First();
                    IEnumerable<string> listOfRecordsToConvert = listOfRecords.Skip(1);

                    Console.WriteLine("Total No. of rows " + listOfRecordsToConvert?.Count());

                    var directionText = (direction == 'y') ? "Last" : "First";
                    if (noOfLines > 0)
                    {
                        if (noOfLines <= listOfRecordsToConvert.Count())
                        {
                            Console.WriteLine($"Fetching {directionText} {noOfLines} records in the file!!");
                            listOfRecordsToConvert = directionText == "First"
                                                                            ? listOfRecordsToConvert.Take(noOfLines ?? 0)
                                                                            : listOfRecordsToConvert.TakeLast(noOfLines ?? 0);
                        }
                        else
                        {
                            throw new Exception($"There are only {listOfRecordsToConvert.Count()} records in the file and you are looking for {noOfLines} records!!");
                        }
                    }

                    // The converted header row is added back to the convertedRecords list
                    var convertedRecords = new List<string>() { ReturnTabbedRecord(headerRecord) };
                    convertedRecords.AddRange(ConvertToTSV(listOfRecordsToConvert));

                    Console.WriteLine("Writing tsv file to " + outputFilePath);
                    File.WriteAllLines(outputFilePath, convertedRecords);

                    Console.WriteLine("Successfully Converted!");
                }
                else
                {
                    Console.WriteLine("File is either empty or not valid");
                }
            }
            else
            {
                Console.WriteLine("File doesnot exists!!");
                throw new FileNotFoundException("File doesnot exists!!");
            }

        }


        /// <summary>
        /// Conversion to the TSV
        /// </summary>
        /// <param name="listOfRecordsToConvert"></param>
        /// <returns></returns>
        public static IEnumerable<string> ConvertToTSV(IEnumerable<string> listOfRecordsToConvert)
        {
            var listOfRecords = new List<string>();

            foreach (string record in listOfRecordsToConvert)
            {
                listOfRecords.Add((string)ReturnTabbedRecord(record));
            }
            return listOfRecords;
        }

        /// <summary>
        /// Returns a tabbed record string
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public static string ReturnTabbedRecord(string record)
        {
            return record.Replace(',', '\t');
        }
    }
}