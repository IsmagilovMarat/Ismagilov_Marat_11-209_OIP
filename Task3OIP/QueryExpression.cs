using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Task3OIP
{
    // Классы для разбора запроса
    public abstract class QueryExpression { }

    public class TermExpression : QueryExpression
    {
        public string Term { get; }
        public TermExpression(string term) => Term = term;
    }

    public class NotExpression : QueryExpression
    {
        public QueryExpression Expression { get; }
        public NotExpression(QueryExpression expr) => Expression = expr;
    }

    public class BinaryExpression : QueryExpression
    {
        public QueryExpression Left { get; }
        public QueryExpression Right { get; }
        public string Operator { get; }

        public BinaryExpression(QueryExpression left, string op, QueryExpression right)
        {
            Left = left;
            Operator = op;
            Right = right;
        }
    }

    public class QueryParser
    {
        private int _position;
        private List<string> _tokens;

        public QueryExpression Parse(string query)
        {
            _position = 0;
            _tokens = TokenizeQuery(query);

            var result = ParseExpression();

            return result;
        }

        private List<string> TokenizeQuery(string query)
        {
            var tokens = new List<string>();
            var pattern = @"\(|\)|AND|OR|NOT|\b\w+(?:'\w+)?\b";

            var matches = Regex.Matches(query, pattern, RegexOptions.IgnoreCase);

            foreach (Match match in matches)
            {
                tokens.Add(match.Value);
            }

            return tokens;
        }

        private QueryExpression ParseExpression()
        {
            return ParseOr();
        }

        private QueryExpression ParseOr()
        {
            var left = ParseAnd();

            while (_position < _tokens.Count && _tokens[_position].Equals("OR", StringComparison.OrdinalIgnoreCase))
            {
                _position++;
                var right = ParseAnd();
                left = new BinaryExpression(left, "OR", right);
            }

            return left;
        }

        private QueryExpression ParseAnd()
        {
            var left = ParseNot();

            while (_position < _tokens.Count && _tokens[_position].Equals("AND", StringComparison.OrdinalIgnoreCase))
            {
                _position++;
                var right = ParseNot();
                left = new BinaryExpression(left, "AND", right);
            }

            return left;
        }

        private QueryExpression ParseNot()
        {
            if (_position < _tokens.Count && _tokens[_position].Equals("NOT", StringComparison.OrdinalIgnoreCase))
            {
                _position++;
                var expr = ParsePrimary();
                return new NotExpression(expr);
            }

            return ParsePrimary();
        }

        private QueryExpression ParsePrimary()
        {
            if (_position >= _tokens.Count)
                throw new InvalidOperationException("Неожиданный конец запроса");

            string token = _tokens[_position];

            if (token == "(")
            {
                _position++;
                var expr = ParseExpression();

                if (_position >= _tokens.Count || _tokens[_position] != ")")
                    throw new InvalidOperationException("Ожидалась закрывающая скобка ')'");

                _position++;
                return expr;
            }
            _position++;
            return new TermExpression(token);
        }
    }

}
