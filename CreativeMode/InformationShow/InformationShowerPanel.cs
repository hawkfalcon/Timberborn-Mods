using TimberApi.UiBuilderSystem;
using Timberborn.SingletonSystem;
using Timberborn.UILayoutSystem;
using UnityEngine.UIElements;

namespace CreativeMode.InformationShow
{
    public class InformationShowerPanel : ILoadableSingleton
    {
        private Label _label;
        private readonly UILayout _gameLayout;
        private readonly UIBuilder _builder;
        private VisualElement _root;
        
        private const int _panelOrder = 8;

        public InformationShowerPanel(UILayout gameLayout, UIBuilder uIBuilder)
        {
            _gameLayout = gameLayout;
            _builder = uIBuilder;
        }

        public void Load()
        {
            _root =
                _builder.CreateComponentBuilder()
                    .CreateVisualElement()
                    .AddClass("top-right-item")
                    .AddClass("square-large--green")
                    .SetFlexDirection(FlexDirection.Row)
                    .SetFlexWrap(Wrap.Wrap)
                    .SetJustifyContent(Justify.Center)
                    .AddComponent(builder => builder.AddComponent(_builder.CreateComponentBuilder()
                        .CreateLabel()
                        .AddClass("text--centered")
                        .AddClass("text--yellow")
                        .AddClass("date-panel__text")
                        .AddClass("game-text-normal")
                        .SetName("InformationShowerPanel")
                        .Build()))
                    .BuildAndInitialize();

            _label = _root.Q<Label>("InformationShowerPanel");
            _gameLayout.AddTopRight(_root, _panelOrder);
        }
        
        public void Text(string text)
        {
            _label.text = text;
        }
    }
}