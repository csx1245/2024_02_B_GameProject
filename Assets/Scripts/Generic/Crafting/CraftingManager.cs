using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Weapon;

namespace MyGame.CraftingSystem
{
    public class CraftingManager : MonoBehaviour
    {
        private static CraftingManager instance;
        public static CraftingManager Instance => instance;

        private Dictionary<string, Recipe> recipes = new Dictionary<string, Recipe>();
        private Inventory<IItem> playerInventory;
        private InventoryManager inventoryManager;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            //InventoryManager 찾기

            inventoryManager = FindObjectOfType<InventoryManager>();
            if(inventoryManager != null)
            {
                playerInventory = inventoryManager.GetInventory();
            }
            else
            {
                Debug.LogError("InventoryManager not found");
            }
        }

        private void CreateSwordRecipe()
        {
            var ironSword = new Weapon("Iron Sword", 1001, 10);
            var recipe = new Recipe("RECIPE_IRON_SWORD", ironSword, 1);
            recipe.AddRequiredMaterial(101, 2);         //Iron Ingot x 2
            recipe.AddRequiredMaterial(102, 1);         //Wood x 1
            recipes.Add(recipe.recipeId, recipe);
        }

        private void CreatePotionRecipe()
        {
            var healthPotion = new HealthPotion("Health Potion", 2001, 50);
            var recipe = new Recipe("RECIPE_HEALTH_POTION", healthPotion, 1);
            recipe.AddRequiredMaterial(201, 2);         //Herb x 2
            recipe.AddRequiredMaterial(202, 1);         //Water x 1
            recipes.Add(recipe.recipeId, recipe);
        }

        public bool TryCraft(string recipeId)                    //조합 시도
        {
            if(!recipes.TryGetValue(recipeId, out Recipe recipe))
                return false;
            
            if(!CheckMaterials(recipe))
                return false;

            ConsumeMaterials(recipe);
            CreateResult(recipe);

            return true;
        }

        private bool CheckMaterials(Recipe recipe)                  //재료 확인 함수
        {
            playerInventory = inventoryManager.GetInventory();

            foreach (var material in recipe.requiredMaterials)
            {
                if (!playerInventory.HasEnough(material.Key, material.Value))
                    return false;
            }
            return true;
        }

        private void ConsumeMaterials(Recipe recipe)                //조합 시 레시피에 있는 필요 아이템을 제거
        {
            foreach(var material in recipe.requiredMaterials)
            {
                playerInventory.RemoveItems(material.Key, material.Value);
            }
        }

        private void CreateResult(Recipe recipe)                    //조합 완료 시 인벤토리에 아이템 추가
        {
            playerInventory.AddItem(recipe.resultItem);
        }

        public List<Recipe> GetAvailableRecipes()                   //가능한 레시피 리턴하는 함수
        {
            return new List<Recipe>(recipes.Values);
        }

    }

}
