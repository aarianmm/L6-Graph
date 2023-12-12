using System;
using System.Text.RegularExpressions;

namespace L6_Graph
{
	public class FormattingData
	{
        private string projectPath;
        string submissionsCSV;
        public string SubmissionsCSV { get { return submissionsCSV; } }
        string[] l6Names;
        public string[] L6Names { get { return l6Names; } }
        string[] submissionsNames;
        string[] unsubmittedNames;
        HashSet<string> fixedNames;
        Stack<string> wrongNames;
        public FormattingData(string projectPath)
		{
            this.projectPath = projectPath;
            l6Names = populateL6Names();
            submissionsCSV = populateSubmissionsCSV();
            submissionsNames = populateSubmissionNames();
            unsubmittedNames = populateUnsubmittedNames();
            wrongNames = populateWrongNames();
            fixedNames = populateFixedNames();
            applyFixedNames();
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
            //Regex SpacesAtEnd = new Regex(@" (\n|\r)");
            //csv = SpacesAtEnd.Replace(csv, "\n");
            csv = capitaliseCSV(csv);
            csv = csv.Replace('\r', '\n');
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
            Regex completedForm = new Regex(@"([^,]+),.+\n*");
            MatchCollection submissionNameMatches = completedForm.Matches(submissionsCSV);
            string[] names = new string[submissionNameMatches.Count];
            for (int i = 0; i < names.Length; i++)
            {
                names[i] = submissionNameMatches[i].Groups[1].Value;
                //Console.WriteLine(names[i]);
            }
            return names;
        }
        private string[] populateUnsubmittedNames()
        {
            string[] names = new string[l6Names.Length - submissionsNames.Length];
            int index = 0;
            foreach(string name in l6Names)
            {
                if (!submissionsNames.Contains(name))
                {
                    names[index] = name;
                    //Console.WriteLine(index);
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
                if (!l6Names.Contains(match.Groups[1].Value))
                {
                    names.Push(match.Groups[1].Value);
                }
            }
            return names;
        }
        private void displayRatio()
        {
            Console.WriteLine(submissionsNames.Length + ":" + unsubmittedNames.Length + "\n" + l6Names.Length);
        }
        public void displayAllNames()
        {
            for (int i = 0; i < l6Names.Length; i++)
            {
                Console.WriteLine(l6Names[i]) ;
            }
        }
        public void displaySubmittedNames()
        {
            for (int i = 0; i < submissionsNames.Length; i++)
            {
                Console.WriteLine(submissionsNames[i]);
            }
            Console.WriteLine();
        }
        public void displayUnsubmittedNames()
        {
            for (int i = 0; i < unsubmittedNames.Length; i++)
            {
                Console.WriteLine(unsubmittedNames[i]);
            }
            displayRatio();
        }
        public void displayWrongNames() //debugging only
        {
            string[] names = wrongNames.ToArray();
            for (int i = 0; i < names.Length; i++)
            {
                Console.WriteLine(names[i]);
            }
        }
        public void fixWrongNames()
        {
            string[] names = wrongNames.ToArray();
            for (int i = 0; i < names.Length; i++)
            {
                Console.WriteLine(names[i] + " " + (i+1) + "/" + names.Length);
                string fix = "";
                bool accurate = false;
                while (!accurate)
                {
                    fix = Console.ReadLine();
                    if (l6Names.Contains(fix))
                    {
                        accurate = true;
                        Console.Clear();
                    }
                    if (fix == "!")
                    {
                        saveFixedNames();
                        return;
                    }
                }
                submissionsCSV = submissionsCSV.Replace(","+names[i]+",", ","+fix+",");
                submissionsCSV = submissionsCSV.Replace("," + names[i] + "\n", "," + fix + "\n");
                fixedNames.Add(names[i] + ":" + fix);
            }
            saveFixedNames();
        }
        private void applyFixedNames()
        {
            foreach (string fixedName in fixedNames)
            {
                string[] nameAndFixed = fixedName.Split(":");
                submissionsCSV = submissionsCSV.Replace("," + nameAndFixed[0] + ",", "," + nameAndFixed[1] + ",");
                submissionsCSV = submissionsCSV.Replace("," + nameAndFixed[0] + "\n", "," + nameAndFixed[1] + "\n");
                //Console.WriteLine(nameAndFixed[0]);
                //Console.WriteLine(nameAndFixed[1]);
                //Console.WriteLine(submissionsCSV);
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
        public void saveNewCSV()
        {
            StreamWriter CSVWriter = new StreamWriter(projectPath + "/Survey.csv");
            CSVWriter.Write(submissionsCSV);
            CSVWriter.Close();
        }
        private void saveFixedNames()
        {
            StreamWriter FixedNamesWriter = new StreamWriter(projectPath + "/FixedNames.txt",false);
            foreach(string fixedName in fixedNames)
            {
                FixedNamesWriter.WriteLine(fixedName);
            }
            FixedNamesWriter.Close();
        }
        private HashSet<string> populateFixedNames()
        {
            HashSet<string> names = new HashSet<string>();
            StreamReader FixedNamesReader = new StreamReader(projectPath + "/FixedNames.txt");
            while (!FixedNamesReader.EndOfStream)
            {
                names.Add(FixedNamesReader.ReadLine());
            }
            FixedNamesReader.Close();
            return names;
        }
    }
}

