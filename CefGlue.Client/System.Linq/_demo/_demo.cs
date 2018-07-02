using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;
using System.Linq.Dynamic;


namespace Linq4you
{


    class ___demo
    {


        public class cQuestion
        {
            public int iID = 0;
            public string strTitle = "";
            public string strBody = "";
        } // End Class cQuestion
        public class cQuestion2
        {
            public int iID = 0;
            public string strTitle = "";
        } // End Class cQuestion


        static void run()
        {
            List<cQuestion> lsAllQuestions = new List<cQuestion>();

            cQuestion qThisQuestion;
            for (int i = 0; i < 100; ++i)
            {
                qThisQuestion = new cQuestion();
                qThisQuestion.iID = i;
                qThisQuestion.strTitle = "Title " + i.ToString();
                qThisQuestion.strBody = "Body " + i.ToString();
                lsAllQuestions.Add(qThisQuestion);
            } // Next i




            //IQueryable<cQuestion> lsqSelectedQuestions = lsAllQuestions.AsQueryable().OrderBy("iID DESC").Skip(20).Take(10);
            var lsqSelectedQuestions = lsAllQuestions.AsQueryable().OrderBy("iID DESC").Skip(19).Take(11);
            var l2 = lsAllQuestions.AsQueryable().Select("new(iID,strTitle)").OrderBy("iID DESC").Skip(19).Take(11).Cast<dynamic>().AsEnumerable().ToList();


            Console.WriteLine("Example 1:");
            foreach (cQuestion q in lsqSelectedQuestions)
            {
                Console.WriteLine(q.strTitle);
            } // Next q


            Console.WriteLine(Environment.NewLine);
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine(new string('-', 60));

            Console.WriteLine("Example 2:");

            string[] names = { "John", "Peter", "Joe", "Patrick", "Donald", "Eric" };

            IEnumerable<string> namesWithFiveCharacters =
                                        from name in names
                                        where name.Length < 5
                                        select name;

            foreach (var name in namesWithFiveCharacters)
                Console.WriteLine(name);

            Console.WriteLine(Environment.NewLine);
            Console.WriteLine(" --- Press any key to continue --- ");
            Console.ReadKey();
        } // End Sub Main


    } // End Class Program


} // End Namespace Linq4you
