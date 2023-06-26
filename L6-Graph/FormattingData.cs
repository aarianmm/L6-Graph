using System;
using System.Text.RegularExpressions;

namespace L6_Graph
{
	public class FormattingData
	{
        private string projectPath;
        string submissionsCSV;
        string[] L6Names;
        string[] submissionsNames;
        string[] unsubmittedNames;
        Stack<string> wrongNames;
        public FormattingData(string projectPath)
		{
            this.projectPath = projectPath;
            L6Names = populateL6Names();
            submissionsCSV = populateSubmissionsCSV();
            submissionsNames = populateSubmissionNames();
            unsubmittedNames = populateUnsubmittedNames();
            wrongNames = populateWrongNames();
        }
        private string[] populateL6Names()
        {
            StreamReader L6NamesReader = new StreamReader(projectPath + "/L6-Names.txt");
            string[] names = L6NamesReader.ReadToEnd().Split('\n');
            L6NamesReader.Close();
            return names;
        }
        private string populateSubmissionsCSV()
        {
            StreamReader SubmissionsReader = new StreamReader(projectPath + "/Survey.csv");
            string csv = SubmissionsReader.ReadToEnd();
            SubmissionsReader.Close();
            Regex SpacesAfterName = new Regex(@" ,");
            csv = SpacesAfterName.Replace(csv, ",");
            csv = capitaliseCSV(csv);
            return csv;
        }
        private string capitaliseCSV(string csv)
        {
            for(int i=1; i<csv.Length; i++)
            {
                if (char.IsLower(csv[i]) && (csv[i - 1] == ',' || csv[i - 1] == ' '))
                {
                    csv = csv.Substring(0, i) + char.ToUpper(csv[i]) + csv.Substring(i + 1);
                }
            }
            return csv;
        }
        private string[] populateSubmissionNames()
        {
            Regex completedForm = new Regex(@"([^,]+),.+\n?");
            MatchCollection submissionNameMatches = completedForm.Matches(submissionsCSV);

            string[] names = new string[submissionNameMatches.Count];
            for (int i = 0; i < names.Length; i++)
            {
                names[i] = submissionNameMatches[i].Groups[1].Value;
            }
            return names;
        }
        private string[] populateUnsubmittedNames()
        {
            string[] names = new string[L6Names.Length - submissionsNames.Length];
            int index = 0;
            foreach(string name in L6Names)
            {
                if (!submissionsNames.Contains(name))
                {
                    names[index] = name;
                    index++;
                }
            }
            return names;
        }
        private Stack<string> populateWrongNames()
        {
            Regex AllNamesReader = new Regex(@"([^,\n\r]+)");
            Stack<string> names = new Stack<string>();
            MatchCollection matches = AllNamesReader.Matches(submissionsCSV);
            foreach(Match match in matches)
            {
                if (!L6Names.Contains(match.Groups[1].Value))
                {
                    names.Push(match.Groups[1].Value);
                }
            }
            return names;
        }

        public void displayAllNames()
        {
            for (int i = 0; i < L6Names.Length; i++)
            {
                Console.WriteLine(L6Names[i]);
            }
        }
        public void displaySubmissionNames()
        {
            for (int i = 0; i < submissionsNames.Length; i++)
            {
                Console.WriteLine(submissionsNames[i]);
            }
        }
        public void displayUnsubmittedNames()
        {
            for (int i = 0; i < unsubmittedNames.Length; i++)
            {
                Console.WriteLine(unsubmittedNames[i]);
            }
        }
        public void displayWrongNames() //debugging only
        {
            string[] names = wrongNames.ToArray();
            for (int i = 0; i < names.Length; i++)
            {
                Console.WriteLine(names[i]);
            }
        }
        public void saveUnsubmittedNames()
        {
            StreamWriter UnsubmittedNamesWriter = new StreamWriter(projectPath + "/UnsubmittedNames.txt", false);
            for (int i = 0; i < unsubmittedNames.Length; i++)
            {
                UnsubmittedNamesWriter.WriteLine(unsubmittedNames[i]);
            }
            UnsubmittedNamesWriter.Close();
        }
        public void saveUnsubmittedNames(string filePath)
        {
            StreamWriter UnsubmittedNamesWriter = new StreamWriter(filePath + "/UnsubmittedNames.txt", false);
            for (int i = 0; i < unsubmittedNames.Length; i++)
            {
                UnsubmittedNamesWriter.WriteLine(unsubmittedNames[i]);
            }
            UnsubmittedNamesWriter.Close();
        }


    }
}

