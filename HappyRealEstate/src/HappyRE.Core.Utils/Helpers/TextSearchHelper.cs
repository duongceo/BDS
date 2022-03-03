using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyRE.Core.Utils.Helpers
{
    public static class TextSearchHelper
    {
        public static string ToContainsQuery(this string query)
        {
            if (string.IsNullOrEmpty(query)) return query;

            string containsQuery = string.Empty;

            var terms = query.Split(new[] { ' ' }, StringSplitOptions.None);

            if (terms.Length > 1)
            {
                for (int i = 0; i < terms.Length; i++)
                {
                    string term = terms[i].Trim();

                    // Add wildcard term, e.g. - "term*". The reason to add wildcard is because we want
                    // to allow search by partially entered name parts (partially entered first name and/or
                    // partially entered last name, etc).
                    containsQuery += "\"" + term + "*\"";

                    // If it's not the last term.
                    if (i < terms.Length - 1)
                    {
                        // We want all terms inside user query to match.
                        containsQuery += " AND ";
                    }
                }

                containsQuery = containsQuery.Trim();
            }
            else
            {
                containsQuery = "\"" + query + "*\"";
            }

            return containsQuery;
        }

        public static string ToEqualQuery(this string query)
        {
            if (string.IsNullOrEmpty(query)) return query;

            string equalQuery = string.Empty;

            var terms = query.Split(new[] { ' ' }, StringSplitOptions.None);

            if (terms.Length > 1)
            {
                for (int i = 0; i < terms.Length; i++)
                {
                    string term = terms[i].Trim();

                    // Add wildcard term, e.g. - "term*". The reason to add wildcard is because we want
                    // to allow search by partially entered name parts (partially entered first name and/or
                    // partially entered last name, etc).
                    equalQuery += term;

                    // If it's not the last term.
                    if (i < terms.Length - 1)
                    {
                        // We want all terms inside user query to match.
                        equalQuery += " AND ";
                    }
                }

                equalQuery = equalQuery.Trim();
            }
            else
            {
                equalQuery = query.Trim();
            }

            return equalQuery;
        }
    }
}
