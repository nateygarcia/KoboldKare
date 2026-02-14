using System.IO;
using UnityEditor.AssetImporters;
using UnityEngine;


[ScriptedImporter(1, "tdlg")]
public class DialogueImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
        TextAsset textAsset = new TextAsset(File.ReadAllText(ctx.assetPath));
        ctx.AddObjectToAsset("text", textAsset);
        ctx.SetMainObject(textAsset);
    }
}

