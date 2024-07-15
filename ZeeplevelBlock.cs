using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

public class ZeeplevelBlock
{
    public int BlockID { get; private set; }
    public Vector3 Position { get; private set; }
    public Vector3 Rotation { get; private set; }
    public Vector3 Scale { get; private set; }
    public List<float> Properties { get; private set; }

    public ZeeplevelBlock()
    {
        BlockID = -1;
        Position = Vector3.zero;
        Rotation = Vector3.zero;
        Scale = Vector3.one;
        Properties = new List<float> { 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    }

    public ZeeplevelBlock(string csvData)
    {
        ReadCSVString(csvData);
    }

    private void ReadCSVString(string data)
    {
        string[] values = data.Split(',');

        if (values.Length != 38)
        {
            return;
        }

        BlockID = ParseInt(values[0]);
        Position = new Vector3(ParseFloat(values[1]), ParseFloat(values[2]), ParseFloat(values[3]));
        Rotation = new Vector3(ParseFloat(values[4]), ParseFloat(values[5]), ParseFloat(values[6]));
        Scale = new Vector3(ParseFloat(values[7]), ParseFloat(values[8]), ParseFloat(values[9]));

        Properties = new List<float>();
        Properties.Add(Position.x);
        Properties.Add(Position.y);
        Properties.Add(Position.z);
        Properties.Add(Rotation.x);
        Properties.Add(Rotation.y);
        Properties.Add(Rotation.z);
        Properties.Add(Scale.x);
        Properties.Add(Scale.y);
        Properties.Add(Scale.z);

        for (int i = 10; i < values.Length; i++)
        {
            Properties.Add(ParseFloat(values[i]));
        }
    }

    private int ParseInt(string value)
    {
        return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result) ? result : -1;
    }

    private float ParseFloat(string value)
    {
        return float.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out float result) ? result : 0.0f;
    }

    public string ToCSV()
    {
        StringBuilder csvBuilder = new StringBuilder();

        // First part: BlockID, Position, Rotation, Scale
        csvBuilder.Append($"{BlockID},{Position.x.ToString(CultureInfo.InvariantCulture)},{Position.y.ToString(CultureInfo.InvariantCulture)},{Position.z.ToString(CultureInfo.InvariantCulture)},");
        csvBuilder.Append($"{Rotation.x.ToString(CultureInfo.InvariantCulture)},{Rotation.y.ToString(CultureInfo.InvariantCulture)},{Rotation.z.ToString(CultureInfo.InvariantCulture)},");
        csvBuilder.Append($"{Scale.x.ToString(CultureInfo.InvariantCulture)},{Scale.y.ToString(CultureInfo.InvariantCulture)},{Scale.z.ToString(CultureInfo.InvariantCulture)},");

        // Properties part
        for (int i = 10; i < Properties.Count; i++)
        {
            csvBuilder.Append(Properties[i].ToString(CultureInfo.InvariantCulture));
            if (i < Properties.Count - 1)
            {
                csvBuilder.Append(",");
            }
        }

        return csvBuilder.ToString();
    }
}
