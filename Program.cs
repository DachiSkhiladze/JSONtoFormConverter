using NBGParser;
using System.Text.Json;

public class Program
{

    static int br = 0; // [ open count
    static int ar = 0; // { open count
    static int checkPointBr = br;
    static int checkPointAr = ar;
    public static async Task Main(string[] args)
    {
        List<SchemeJSON> schemes = new List<SchemeJSON>();
        string raw = System.IO.File.ReadAllText(@"C:\Users\Dachi\source\repos\NBGParser\content.json");
        raw = raw.Replace(System.Environment.NewLine, "");
        var i = 0;
        while (i < raw.Length)
        {
            if (raw[i] == '[')
            {
                if(ar == 0)
                {
                    schemes.Last().type = "list";
                }
            }
            else if(raw[i] == '"')
            {
                if(br == 1 && ar == 0)
                {
                    var scheme = new SchemeJSON();
                    AddTypeName(ref i, ref scheme, raw);
                    schemes.Add(scheme);
                }
                else
                {
                    var schemeFields = schemes.Last().fields;
                    if (schemeFields == null)
                    {
                        schemeFields = new List<Field>();
                    }
                    AddFields(ref i, ref schemeFields, raw);
                    schemes.Last().fields = schemeFields;
                }
            }

            MoveIndex(ref i, raw);
        }
        var result = JsonSerializer.Serialize(schemes);

        File.WriteAllText(@"C:\Users\Dachi\source\repos\NBGParser\Result.json", result);
    }

    public static void MoveIndex(ref int i, string raw, int count = 1)
    {
        if (raw[i] == '[')
        {
            ar++;
        }
        else if (raw[i] == '{')
        {
            br++;
        }
        else if (raw[i] == '}')
        {
            br--;
        }
        else if (raw[i] == ']')
        {
            ar--;
        }
        i += count;
    }

    public static void AddFields(ref int i, ref List<Field> scheme, string raw)
    {
        Field field = new Field();
        while (raw[i] != '"')
        {
            MoveIndex(ref i, raw);
        }
        MoveIndex(ref i, raw);
        while (raw[i] != '"')// adding field id
        {
            field.id += raw[i]; 
            MoveIndex(ref i, raw);

        }
        var typeDetect = raw[i + 3];
        MoveIndex(ref i, raw, 3);
        if (typeDetect == '{') // multiselector or text
        {
            if (field.id == "Name" || field.id == "QuestionDifficulties")
            {
                field.type = "text";
                var tmp = ar;
                while (true)
                {
                    if (raw[i] == '}' && tmp == ar)
                    {
                        MoveIndex(ref i, raw);
                        break;
                    }
                    MoveIndex(ref i, raw);
                }
            }
            else
            {
                field.type = "form";
            }
        }
        else if(typeDetect == '[') // arr
        {
            field.type = "list";
            var schemeFields = new List<Field>();
            checkPointAr = ar;
            checkPointBr = br;
            AddFields(ref i, ref schemeFields, raw);
            field.fields = schemeFields;
        }
        else if(typeDetect == '"') // text or guid
        {
            DetectStringOrGuid(ref i, ref field, raw);
        }
        else if(typeDetect == 'n') // null
        {
            field.type = "text";
        }
        else // number
        {
            field.type = "number";
        }

        if(checkPointBr == br && checkPointAr == ar)
        {
            scheme.Add(field);
        }
        else
        {
            MoveIndex(ref i, raw);
            scheme.Add(field);
            AddFields(ref i, ref scheme, raw);
        }
    }

    public static void DetectStringOrGuid(ref int i, ref Field field, string raw)
    {
        string inputString = "";
        var current = raw[i];
        MoveIndex(ref i, raw);
        while (raw[i] != '"')
        {
            inputString += raw[i];
            MoveIndex(ref i, raw);
        }
        MoveIndex(ref i, raw);
        Guid x;
        bool isValid = Guid.TryParse(inputString, out x);
        if (isValid)
        {
            field.type = "Guid";
        }
        else
        {
            field.type = "text";
        }
    }

    public static void AddTypeName(ref int i, ref SchemeJSON scheme, string raw)
    {
        while (true)
        {
            MoveIndex(ref i, raw);
            if (raw[i] == '"')
            {
                return;
            }
            scheme.id += raw[i];
        }
    }
}