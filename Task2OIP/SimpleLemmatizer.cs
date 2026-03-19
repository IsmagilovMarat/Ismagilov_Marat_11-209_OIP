using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task1OIP
{
    public class SimpleLemmatizer
    {
        private readonly Dictionary<string, string> _irregularVerbs;
        private readonly Dictionary<string, string> _irregularPlurals;

        public SimpleLemmatizer()
        {
            // Неправильные глаголы
            _irregularVerbs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                // Быть
                {"am", "be"}, {"is", "be"}, {"are", "be"}, {"was", "be"}, {"were", "be"}, {"been", "be"}, {"being", "be"},
                // Иметь
                {"has", "have"}, {"had", "have"}, {"having", "have"},
                // Делать
                {"does", "do"}, {"did", "do"}, {"doing", "do"}, {"done", "do"},
                // Идти
                {"goes", "go"}, {"went", "go"}, {"gone", "go"}, {"going", "go"},
                // Видеть
                {"sees", "see"}, {"saw", "see"}, {"seen", "see"}, {"seeing", "see"},
                // Говорить
                {"says", "say"}, {"said", "say"}, {"saying", "say"},
                // Получать
                {"gets", "get"}, {"got", "get"}, {"gotten", "get"}, {"getting", "get"},
                // Делать/создавать
                {"makes", "make"}, {"made", "make"}, {"making", "make"},
                // Знать
                {"knows", "know"}, {"knew", "know"}, {"known", "know"}, {"knowing", "know"},
                // Думать
                {"thinks", "think"}, {"thought", "think"}, {"thinking", "think"},
                // Брать
                {"takes", "take"}, {"took", "take"}, {"taken", "take"}, {"taking", "take"},
                // Приходить
                {"comes", "come"}, {"came", "come"}, {"coming", "come"},
                // Находить
                {"finds", "find"}, {"found", "find"}, {"finding", "find"},
                // Давать
                {"gives", "give"}, {"gave", "give"}, {"given", "give"}, {"giving", "give"},
                // Рассказывать
                {"tells", "tell"}, {"told", "tell"}, {"telling", "tell"},
                // Продавать
                {"sells", "sell"}, {"sold", "sell"}, {"selling", "sell"},
                // Покупать
                {"buys", "buy"}, {"bought", "buy"}, {"buying", "buy"},
                // Ловить
                {"catches", "catch"}, {"caught", "catch"}, {"catching", "catch"},
                // Учить
                {"teaches", "teach"}, {"taught", "teach"}, {"teaching", "teach"},
                // Писать
                {"writes", "write"}, {"wrote", "write"}, {"written", "write"}, {"writing", "write"},
                // Читать
                {"reads", "read"}, {"reading", "read"},
                // Бежать
                {"runs", "run"}, {"ran", "run"}, {"running", "run"},
                // Сидеть
                {"sits", "sit"}, {"sat", "sit"}, {"sitting", "sit"},
                // Стоять
                {"stands", "stand"}, {"stood", "stand"}, {"standing", "stand"},
                // Понимать
                {"understands", "understand"}, {"understood", "understand"}, {"understanding", "understand"},
                // Чувствовать
                {"feels", "feel"}, {"felt", "feel"}, {"feeling", "feel"},
                // Держать
                {"holds", "hold"}, {"held", "hold"}, {"holding", "hold"},
                // Встречать
                {"meets", "meet"}, {"met", "meet"}, {"meeting", "meet"},
                // Спать
                {"sleeps", "sleep"}, {"slept", "sleep"}, {"sleeping", "sleep"},
                // Говорить
                {"speaks", "speak"}, {"spoke", "speak"}, {"spoken", "speak"}, {"speaking", "speak"},
                // Летать
                {"flies", "fly"}, {"flew", "fly"}, {"flown", "fly"}, {"flying", "fly"}
            };

            // Неправильные множественные числа
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

        private string GetLemma(string word)
        {
            string lower = word.ToLower();

            // Проверяем неправильные глаголы
            if (_irregularVerbs.ContainsKey(lower))
                return _irregularVerbs[lower];

            // Проверяем неправильные множественные числа
            if (_irregularPlurals.ContainsKey(lower))
                return _irregularPlurals[lower];

            // Правила для существительных во множественном числе
            if (lower.EndsWith("ies") && lower.Length > 4)
            {
                // "cities" -> "city", "babies" -> "baby"
                return lower.Substring(0, lower.Length - 3) + "y";
            }
            if (lower.EndsWith("es") && lower.Length > 3)
            {
                // "boxes" -> "box", "churches" -> "church"
                if (lower.EndsWith("ses") || lower.EndsWith("ches") || lower.EndsWith("shes") ||
                    lower.EndsWith("xes") || lower.EndsWith("zes"))
                {
                    return lower.Substring(0, lower.Length - 2);
                }
            }
            if (lower.EndsWith("s") && !lower.EndsWith("ss") && lower.Length > 2)
            {
                // "cats" -> "cat", "dogs" -> "dog" (но не "grass" -> "gras")
                return lower.Substring(0, lower.Length - 1);
            }

            // Правила для глаголов в третьем лице
            if (lower.EndsWith("ies") && lower.Length > 4)
            {
                // "carries" -> "carry"
                return lower.Substring(0, lower.Length - 3) + "y";
            }
            if (lower.EndsWith("es") && lower.Length > 3)
            {
                // "watches" -> "watch"
                if (lower.EndsWith("ches") || lower.EndsWith("shes") || lower.EndsWith("sses"))
                {
                    return lower.Substring(0, lower.Length - 2);
                }
            }
            if (lower.EndsWith("ed") && lower.Length > 3)
            {
                // "walked" -> "walk", "studied" -> "study"
                if (lower.EndsWith("ied"))
                {
                    return lower.Substring(0, lower.Length - 3) + "y";
                }
                if (lower.Length > 4 && IsConsonant(lower[lower.Length - 4]) &&
                    "aeiou".IndexOf(lower[lower.Length - 3]) >= 0 &&
                    IsConsonant(lower[lower.Length - 2]))
                {
                    // Удвоение согласной: "stopped" -> "stop"
                    return lower.Substring(0, lower.Length - 3);
                }
                return lower.Substring(0, lower.Length - (lower.EndsWith("ied") ? 3 : 2));
            }
            if (lower.EndsWith("ing") && lower.Length > 4)
            {
                // "running" -> "run", "writing" -> "write"
                string stem = lower.Substring(0, lower.Length - 3);

                // Проверка на удвоение согласной
                if (stem.Length > 1 && IsConsonant(stem[stem.Length - 1]) &&
                    stem[stem.Length - 1] == stem[stem.Length - 2])
                {
                    return stem.Substring(0, stem.Length - 1);
                }

                // "taking" -> "take"
                return stem + "e";
            }

            return lower;
        }

        private bool IsConsonant(char c)
        {
            return !"aeiou".Contains(char.ToLower(c));
        }
    }
}
