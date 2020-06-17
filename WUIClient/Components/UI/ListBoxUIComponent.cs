using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WUIShared.Objects;

namespace WUIClient.Components.UI {
    public class ListBoxUIComponent : GameObject {
        public bool screenPosition = false;
        private List<TextRenderer> items;
        public Texture2D texture;
        public int itemsPerPage = 10;
        public int width = 100;
        public int heightOfOneItem = 25;

        public delegate void ItemSelectionDelegate(ListBoxUIComponent sender, int index, string item);

        public event ItemSelectionDelegate OnItemSelectionChange;

        public ListBoxUIComponent() : base(Objects.ListBoxUIComponent, false) {
            items = new List<TextRenderer>();
        }

        public void Clear() {
            foreach (var item in items) RemoveChild(item.Parent);
        }

        public void AddItem(string text) {
            TextRenderer textRenderer;
            MouseClickableComponent mouseClickable = new MouseClickableComponent(screenPosition);
            GameObject item = new GameObject();
            item.AddChild(new RawTextureRenderer() { texture = texture });
            item.AddChild(textRenderer = new TextRenderer(text, Color.Black));
            item.AddChild(mouseClickable);
            item.AddChild(new ButtonComponent());

            int index = items.Count;
            mouseClickable.mouseClickable.OnMouseLeftClickDown += MouseClickable_OnMouseLeftClickDown;

            item.transform.LocalPosition = new Vector2(0, items.Count * heightOfOneItem);
            
            item.transform.Size = new Vector2(width, heightOfOneItem);
            items.Add(textRenderer);
            AddChild(item);
            
            void MouseClickable_OnMouseLeftClickDown(GameObject sender) {
                OnItemSelectionChange?.Invoke(this, index, sender.Parent.GetFirst<TextRenderer>().text);
            }
        }


        public string this[int id] {
            get => items[id].text;
        }
    }
}
