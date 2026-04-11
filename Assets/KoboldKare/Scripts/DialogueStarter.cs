using TerribleDialogue;
using TerribleDialogue.Model;
using Sprache;
using System.Collections;
using UnityEngine;

public class DialogueStarter : GenericUsable {
    private const string SET_PREFIX = "star";


    [SerializeField] private Sprite useSprite;
    [SerializeField] private GameObject speechBackgroundBubble;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private TMPro.TMP_Text text;
    [SerializeField]
    private AudioPack sansUndertaleVocals;

    [SerializeField] private TextAsset dialogue;

    private AudioSource source;
    private WaitForSeconds textDelay;
    private WaitForSeconds lineDelay;
    private DialogueEngine engine;
        
    private bool talking = false;

    public override Sprite GetSprite(Kobold k) {
        return useSprite;
    }

    private void Awake() {
        textDelay = new WaitForSeconds(0.16f);
        lineDelay = new WaitForSeconds(3f);
        if (source == null) {
            source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.maxDistance = 12f;
            source.minDistance = 8f;
            source.volume = 1f;
            source.rolloffMode = AudioRolloffMode.Linear;
            source.spatialBlend = 1f;
            source.loop = false;
        }
        source.enabled = false;

        engine = new DialogueEngine(DialogueGrammar.Dialogue.Parse(dialogue.text), UnityEngine.Random.Range, SET_PREFIX+"0");
    }

    public override bool CanUse(Kobold k) {
        return !talking;
    }

    public override void Use() {
        base.Use();
        StartCoroutine(Talk());
    }

    private IEnumerator Talk() {
        while (talking) {
            yield return null;
        }

        speechBackgroundBubble.SetActive(true);

        source.enabled = true;
        animator.SetTrigger("Talk");
        talking = true;

        string set = "";
        int bestValue = -1;
        foreach(string id in engine.DialogueObject.Sets.Keys)
        {
            int requiredStars = int.Parse(id.Substring(SET_PREFIX.Length));
            if(ObjectiveManager.GetStars() >= requiredStars && requiredStars > bestValue)
            {
                bestValue = requiredStars;
                set = id;
            }
        }

        // Don't reset the state
        if(engine.CurrentSetId != set)
            engine.SetSet(set);

        engine.Step();
        while(engine.HasLine && !engine.IsDialogueOver) {
            float startTime = Time.time;
            string targetString = engine.CurrentLine.Text;
            float duration = 0.025f*targetString.Length;
            text.text = targetString;
            text.maxVisibleCharacters = 0;
            while (Time.time < startTime + duration) {
                float t = (Time.time - startTime) / duration;
                text.maxVisibleCharacters = Mathf.RoundToInt(targetString.Length * t);
                sansUndertaleVocals.Play(source);
                yield return textDelay;
            }
            text.maxVisibleCharacters = targetString.Length;
            sansUndertaleVocals.Play(source);
            yield return lineDelay;
            text.text = "";

            engine.Step();
        }

        source.enabled = false;
        talking = false;
        speechBackgroundBubble.SetActive(false);
    }
}
