using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EndlessRails
{
    /// <summary>
    /// Character select screen: lists all CharacterDefinition assets, shows
    /// prices, unlocks with coins and swaps the active runner.
    /// </summary>
    public class CharacterSelectMenu : MonoBehaviour
    {
        [SerializeField] private CharacterDefinition[] characters = new CharacterDefinition[0];
        [SerializeField] private Transform listRoot;
        [SerializeField] private Button cardPrefab;
        [SerializeField] private Text coinsText;
        [SerializeField] private Text statusText;

        private readonly List<Button> _spawnedCards = new List<Button>(8);

        private void Awake()
        {
            if (coinsText == null) coinsText = transform.FindDeep<Text>("CoinsText");
            if (statusText == null) statusText = transform.FindDeep<Text>("StatusText");
        }

        private void OnEnable()
        {
            Rebuild();
        }

        private void Rebuild()
        {
            SaveSystem save = SaveSystem.Instance;
            if (save == null)
            {
                return;
            }

            if (coinsText != null)
            {
                coinsText.text = $"{save.Data.coinsBalance:N0}";
            }

            if (listRoot == null || cardPrefab == null)
            {
                if (statusText != null)
                {
                    statusText.text = "No character cards configured.";
                }
                return;
            }

            foreach (var card in _spawnedCards)
            {
                if (card != null)
                {
                    Destroy(card.gameObject);
                }
            }
            _spawnedCards.Clear();

            foreach (var character in characters)
            {
                if (character == null)
                {
                    continue;
                }
                Button card = Instantiate(cardPrefab, listRoot);
                card.gameObject.SetActive(true);
                _spawnedCards.Add(card);

                bool unlocked = character.coinPrice <= 0 || save.Data.IsCharacterUnlocked(character.id);
                bool selected = save.Data.selectedCharacterId == character.id;

                Text label = card.GetComponentInChildren<Text>();
                if (label != null)
                {
                    label.text = unlocked
                        ? $"{character.displayName}{(selected ? "  [SELECTED]" : "")}"
                        : $"{character.displayName}  -  {character.coinPrice} coins";
                }

                string id = character.id;
                int price = character.coinPrice;
                card.onClick.AddListener(() => OnCardClicked(id, price));
            }
        }

        private void OnCardClicked(string characterId, int price)
        {
            SaveSystem save = SaveSystem.Instance;
            if (save == null)
            {
                return;
            }

            bool unlocked = price <= 0 || save.Data.IsCharacterUnlocked(characterId);
            if (!unlocked)
            {
                if (save.TrySpendCoins(price))
                {
                    save.Data.SetCharacterUnlocked(characterId, true);
                    save.Data.selectedCharacterId = characterId;
                    save.MarkDirty();
                    AudioManager.Instance?.PlayPowerup(Vector3.zero);
                    if (statusText != null)
                    {
                        statusText.text = "Unlocked!";
                    }
                }
                else
                {
                    AudioManager.Instance?.PlayPowerdown(Vector3.zero);
                    if (statusText != null)
                    {
                        statusText.text = "Not enough coins.";
                    }
                }
            }
            else
            {
                save.Data.selectedCharacterId = characterId;
                save.MarkDirty();
                AudioManager.Instance?.PlayButton();
                if (statusText != null)
                {
                    statusText.text = "Selected!";
                }
            }
            ApplySelectionToPlayer();
            Rebuild();
        }

        private void ApplySelectionToPlayer()
        {
            SaveSystem save = SaveSystem.Instance;
            if (save == null)
            {
                return;
            }
            foreach (var character in characters)
            {
                if (character != null && character.id == save.Data.selectedCharacterId)
                {
                    FindFirstObjectByType<ProceduralRunner>()?.SetAppearance(character);
                    return;
                }
            }
        }

        public void OnBack()
        {
            AudioManager.Instance?.PlayButton();
            UIManager.Instance?.Show("MainMenu");
        }
    }
}
