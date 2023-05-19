using System.Linq;
using Avalonia.Automation.Peers;
using Avalonia.Controls.Automation.Peers;
using Avalonia.Controls.Primitives;
using Avalonia.Reactive;
using Avalonia.VisualTree;

namespace Avalonia.Controls
{
    /// <summary>
    /// Represents a button that allows a user to select a single option from a group of options.
    /// </summary>
    public class RadioButton : ToggleButton, IGroupRadioButton
    {
        /// <summary>
        /// Identifies the GroupName dependency property.
        /// </summary>
        public static readonly StyledProperty<string?> GroupNameProperty =
            AvaloniaProperty.Register<RadioButton, string?>(nameof(GroupName));

        private RadioButtonGroupManager? _groupManager;

        /// <summary>
        /// Gets or sets the name that specifies which RadioButton controls are mutually exclusive.
        /// </summary>
        public string? GroupName
        {
            get => GetValue(GroupNameProperty);
            set => SetValue(GroupNameProperty, value);
        }

        bool IGroupRadioButton.IsChecked
        {
            get => IsChecked.GetValueOrDefault();
            set => SetCurrentValue(IsCheckedProperty, value);
        }

        protected override void Toggle()
        {
            if (!IsChecked.GetValueOrDefault())
            {
                SetCurrentValue(IsCheckedProperty, true);
            }
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            if (!string.IsNullOrEmpty(GroupName))
            {
                _groupManager?.Remove(this, GroupName);

                _groupManager = RadioButtonGroupManager.GetOrCreateForRoot(e.Root);

                _groupManager.Add(this);
            }
            base.OnAttachedToVisualTree(e);
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);

            if (!string.IsNullOrEmpty(GroupName))
            {
                _groupManager?.Remove(this, GroupName);
            }
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadioButtonAutomationPeer(this);
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == IsCheckedProperty)
            {
                IsCheckedChanged(change.GetNewValue<bool?>());
            }
            else if (change.Property == GroupNameProperty)
            {
                var (oldValue, newValue) = change.GetOldAndNewValue<string?>();
                OnGroupNameChanged(oldValue, newValue);
            }
        }

        private void OnGroupNameChanged(string? oldGroupName, string? newGroupName)
        {
            if (!string.IsNullOrEmpty(oldGroupName))
            {
                _groupManager?.Remove(this, oldGroupName);
            }
            if (!string.IsNullOrEmpty(newGroupName))
            {
                if (_groupManager == null)
                {
                    _groupManager = RadioButtonGroupManager.GetOrCreateForRoot(this.GetVisualRoot());
                }
                _groupManager.Add(this);
            }
        }

        private new void IsCheckedChanged(bool? value)
        {
            var groupName = GroupName;
            if (string.IsNullOrEmpty(groupName))
            {
                var parent = this.GetVisualParent();

                if (value.GetValueOrDefault() && parent != null)
                {
                    var siblings = parent
                        .GetVisualChildren()
                        .OfType<RadioButton>()
                        .Where(x => x != this && string.IsNullOrEmpty(x.GroupName));

                    foreach (var sibling in siblings)
                    {
                        if (sibling.IsChecked.GetValueOrDefault())
                            sibling.SetCurrentValue(IsCheckedProperty, false);
                    }
                }
            }
            else
            {
                if (value.GetValueOrDefault() && _groupManager != null)
                {
                    _groupManager.SetChecked(this);
                }
            }
        }
    }
}
