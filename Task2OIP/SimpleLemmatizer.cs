public class SimpleLemmatizer
{
    private readonly Dictionary<string, string> _irregularVerbs;
    private readonly Dictionary<string, string> _irregularPlurals;

    public SimpleLemmatizer()
    {
        _irregularVerbs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            {"am", "be"}, {"is", "be"}, {"are", "be"}, {"was", "be"}, {"were", "be"}, {"been", "be"}, {"being", "be"},
            {"has", "have"}, {"had", "have"}, {"having", "have"},
            {"does", "do"}, {"did", "do"}, {"doing", "do"}, {"done", "do"},
            {"goes", "go"}, {"went", "go"}, {"gone", "go"}, {"going", "go"},
            {"sees", "see"}, {"saw", "see"}, {"seen", "see"}, {"seeing", "see"},
            {"says", "say"}, {"said", "say"}, {"saying", "say"},
            {"gets", "get"}, {"got", "get"}, {"gotten", "get"}, {"getting", "get"},
            {"makes", "make"}, {"made", "make"}, {"making", "make"},
            {"knows", "know"}, {"knew", "know"}, {"known", "know"}, {"knowing", "know"},
            {"thinks", "think"}, {"thought", "think"}, {"thinking", "think"},
            {"takes", "take"}, {"took", "take"}, {"taken", "take"}, {"taking", "take"},
            {"comes", "come"}, {"came", "come"}, {"coming", "come"},
            {"finds", "find"}, {"found", "find"}, {"finding", "find"},
            {"gives", "give"}, {"gave", "give"}, {"given", "give"}, {"giving", "give"},
            {"tells", "tell"}, {"told", "tell"}, {"telling", "tell"},
            {"sells", "sell"}, {"sold", "sell"}, {"selling", "sell"},
            {"buys", "buy"}, {"bought", "buy"}, {"buying", "buy"},
            {"catches", "catch"}, {"caught", "catch"}, {"catching", "catch"},
            {"teaches", "teach"}, {"taught", "teach"}, {"teaching", "teach"},
            {"writes", "write"}, {"wrote", "write"}, {"written", "write"}, {"writing", "write"},
            {"reads", "read"}, {"reading", "read"},
            {"runs", "run"}, {"ran", "run"}, {"running", "run"},
            {"sits", "sit"}, {"sat", "sit"}, {"sitting", "sit"},
            {"stands", "stand"}, {"stood", "stand"}, {"standing", "stand"},
            {"understands", "understand"}, {"understood", "understand"}, {"understanding", "understand"},
            {"feels", "feel"}, {"felt", "feel"}, {"feeling", "feel"},
            {"holds", "hold"}, {"held", "hold"}, {"holding", "hold"},
            {"meets", "meet"}, {"met", "meet"}, {"meeting", "meet"},
            {"sleeps", "sleep"}, {"slept", "sleep"}, {"sleeping", "sleep"},
            {"speaks", "speak"}, {"spoke", "speak"}, {"spoken", "speak"}, {"speaking", "speak"},
            {"flies", "fly"}, {"flew", "fly"}, {"flown", "fly"}, {"flying", "fly"}
        };

        _irregularPlurals = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            {"children", "child"}, {"men", "man"}, {"women", "woman"}, {"people", "person"},
            {"mice", "mouse"}, {"lice", "louse"}, {"feet", "foot"}, {"teeth", "tooth"},
            {"geese", "goose"}, {"oxen", "ox"}, {"cacti", "cactus"}, {"fungi", "fungus"},
            {"nuclei", "nucleus"}, {"syllabi", "syllabus"}, {"analyses", "analysis"},
            {"theses", "thesis"}, {"crises", "crisis"}, {"phenomena", "phenomenon"},
            {"data", "datum"}, {"criteria", "criterion"}, {"media", "medium"}
        };
    }

    public string GetLemma(string word)
    {
        string lower = word.ToLower();

        if (_irregularVerbs.ContainsKey(lower))
            return _irregularVerbs[lower];

        if (_irregularPlurals.ContainsKey(lower))
            return _irregularPlurals[lower];

        if (lower.EndsWith("ies") && lower.Length > 4)
            return lower.Substring(0, lower.Length - 3) + "y";

        if (lower.EndsWith("es") && lower.Length > 3)
        {
            if (lower.EndsWith("ses") || lower.EndsWith("ches") || lower.EndsWith("shes") ||
                lower.EndsWith("xes") || lower.EndsWith("zes"))
            {
                return lower.Substring(0, lower.Length - 2);
            }
        }

        if (lower.EndsWith("s") && !lower.EndsWith("ss") && lower.Length > 2)
        {
            return lower.Substring(0, lower.Length - 1);
        }

        if (lower.EndsWith("ed") && lower.Length > 3)
        {
            if (lower.EndsWith("ied"))
                return lower.Substring(0, lower.Length - 3) + "y";

            if (lower.Length > 4 && IsConsonant(lower[lower.Length - 4]) &&
                "aeiou".IndexOf(lower[lower.Length - 3]) >= 0 &&
                IsConsonant(lower[lower.Length - 2]))
            {
                return lower.Substring(0, lower.Length - 3);
            }
            return lower.Substring(0, lower.Length - 2);
        }

        if (lower.EndsWith("ing") && lower.Length > 4)
        {
            string stem = lower.Substring(0, lower.Length - 3);
            if (stem.Length > 1 && IsConsonant(stem[stem.Length - 1]) &&
                stem[stem.Length - 1] == stem[stem.Length - 2])
            {
                return stem.Substring(0, stem.Length - 1);
            }
            return stem + "e";
        }

        return lower;
    }

    private bool IsConsonant(char c)
    {
        return !"aeiou".Contains(char.ToLower(c));
    }

    public Dictionary<string, List<string>> LemmatizeTokens(List<string> tokens)
    {
        var lemmaGroups = new Dictionary<string, List<string>>();

        foreach (var token in tokens)
        {
            string lemma = GetLemma(token);

            if (!lemmaGroups.ContainsKey(lemma))
            {
                lemmaGroups[lemma] = new List<string>();
            }

            if (!lemmaGroups[lemma].Contains(token))
            {
                lemmaGroups[lemma].Add(token);
            }
        }

        return lemmaGroups;
    }
}