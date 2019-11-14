using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
	public Transform InventoryPanel;
	public ItemData ItemData;
	public Item SelectedItem;
	public int SelectedItemIndex;
	private List<Item> Items = new List<Item>();
	private int _inventoryKeyDown
	{
		get
		{
			for (int i = 1; i <= 7; i++)
			{
				if (Input.GetKeyDown(KeyCode.Alpha0 + i)) return i;
			}
			return 0;
		}
	}

	private void Awake()
	{
		SelectedItemIndex = -1;
	}

	private void _changeStackText(int index, int num)
	{
		InventoryPanel.GetChild(index).GetChild(1).GetComponent<TextMeshProUGUI>().text = num.ToString();
		if (num <= 1)
		{
			InventoryPanel.GetChild(index).GetChild(1).gameObject.SetActive(false);
		}
		else
		{
			InventoryPanel.GetChild(index).GetChild(1).gameObject.SetActive(true);
		}
	}

	public void OnAddItem(Item item)
	{
		for (int i = 0; i < Items.Count; i++)
		{
			if (Items[i].GetType().Equals(item.GetType()))
			{
				Items[i].Number++;
				_changeStackText(i, Items[i].Number);
				return;
			}
		}
		for (int i = 0; i < Items.Count; i++)
		{
			if (Items[i].GetType().Equals(typeof(EmptyItem)))
			{
				Items[i] = item;
				InventoryPanel.GetChild(i).GetChild(0).GetComponent<Image>().sprite = item.Sprite;
				InventoryPanel.GetChild(i).GetChild(0).GetComponent<Image>().color = Color.white;
				return;
			}
		}
		Items.Add(item);
		InventoryPanel.GetChild(Items.Count - 1).GetChild(0).GetComponent<Image>().sprite = item.Sprite;
		InventoryPanel.GetChild(Items.Count - 1).GetChild(0).GetComponent<Image>().color = Color.white;
	}

	public void OnUseItemOnce(Item item)
	{
		if (!Items.Contains(item))
		{
			Debug.LogError("The Object Getting Deleted is not in the inventory");
		}

		for (int i = 0; i < Items.Count; i++)
		{
			if (Items[i].GetType().Equals(item.GetType()))
			{
				Items[i].Number--;
				_changeStackText(i, Items[i].Number);
				if (Items[i].Number <= 0)
				{
					_deselectItem(i);
					InventoryPanel.GetChild(i).GetChild(0).GetComponent<Image>().sprite = null;
					InventoryPanel.GetChild(i).GetChild(0).GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);
					Items[i] = new EmptyItem(ItemData);
				}
				return;
			}
		}
	}

	private void _onSelectItem(int index)
	{
		if (index > Items.Count - 1 || Items[index].GetType().Equals(typeof(EmptyItem))) return;
		// First Unselect the last selected item
		if (SelectedItemIndex != -1)
		{
			InventoryPanel.GetChild(SelectedItemIndex).GetComponent<Outline>().enabled = false;
		}
		if (SelectedItemIndex == index)
		{
			SelectedItem = null;
			SelectedItemIndex = -1;
			return;
		}
		SelectedItem = Items[index];
		SelectedItemIndex = index;
		InventoryPanel.GetChild(index).GetComponent<Outline>().enabled = true;
	}

	private void _deselectItem(int index)
	{
		InventoryPanel.GetChild(index).GetComponent<Outline>().enabled = false;
		SelectedItem = null;
		SelectedItemIndex = -1;
	}

	private void Update()
	{
		int pressed = _inventoryKeyDown;
		if (pressed != 0)
		{
			_onSelectItem(pressed - 1);
		}
	}
}
