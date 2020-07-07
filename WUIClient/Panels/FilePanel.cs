using Microsoft.Xna.Framework;
using System.IO;
using WUIClient.Components;
using WUIClient.Components.UI;
using WUIShared.Objects;

namespace WUIClient.Panels {
    public class FilePanel : GameObject {
        private ListBoxUIComponent listboxComponent;
        private int selectedId = -1;

        private Vector2 mouseDownPoint, mouseDownOffset;
        private bool isDragging = false;
        private GameObject dragging_ghost;
        private string[] files;

        public delegate void ItemDropDelegate(FilePanel sender, string fileName);
        public event ItemDropDelegate OnItemDrop;

        public FilePanel() {
            AddChild(listboxComponent = new ListBoxUIComponent() { texture = Game1.instance.UIRect, itemsPerPage = 10, width = 100, heightOfOneItem = 24 });
            listboxComponent.OnItemSelectionChange += ListboxComponent_OnItemSelectionChange;

            dragging_ghost = new GameObject();
            dragging_ghost.AddChild(new RawTextureRenderer() { texture = Game1.instance.UIRect });
            dragging_ghost.AddChild(new TextRenderer("Drag Test", Color.Black));
            dragging_ghost.transform.Size = new Vector2(100, 24);
        }

        private void ListboxComponent_OnItemSelectionChange(ListBoxUIComponent sender, int index, string item) {
            mouseDownPoint = WMouse.Position;
            mouseDownOffset = Vector2.Zero;
            selectedId = index;
        }

        public override void OnUpdate(float deltaTime) {
            base.OnUpdate(deltaTime);
            if(selectedId >= 0) {
                if(!isDragging && (mouseDownPoint - WMouse.Position).LengthSquared() >= 10f) {
                    isDragging = true;
                    dragging_ghost.GetFirst<TextRenderer>().text = listboxComponent[selectedId];
                    AddChild(dragging_ghost);
                }

                if (isDragging)
                    dragging_ghost.transform.Position = WMouse.Position + mouseDownOffset;

                if (WMouse.LeftMouseClickUp()) {
                    if(isDragging) {
                        OnItemDrop?.Invoke(this, files[selectedId]);
                        RemoveChild(dragging_ghost);
                    }
                    isDragging = false;
                    selectedId = -1;
                }    
            }
        }

        public void OpenDirectory(string directory) {
            files = Directory.GetFiles(directory);
            listboxComponent.Clear();
            foreach (var item in files) {
                var sp = item.Split('\\');
                listboxComponent.AddItem(sp[sp.Length - 1]);
            }
        }

    }
}
