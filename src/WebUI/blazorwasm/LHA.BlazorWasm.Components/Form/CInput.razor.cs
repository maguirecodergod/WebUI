using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using LHA.BlazorWasm.Shared.Abstractions.Localization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace LHA.BlazorWasm.Components.Form
{
    public partial class CInput<TValue> : InputBase<TValue>
    {
        [Inject] private ILocalizationService _localizer { get; set; } = default!;

        [Parameter] public string Type { get; set; } = "text";
        [Parameter] public string? Placeholder { get; set; }
        [Parameter] public string? Class { get; set; }
        [Parameter] public string? Style { get; set; }
        [Parameter] public string? AriaLabel { get; set; }

        [Parameter] public CInputSize Size { get; set; } = CInputSize.Medium;
        [Parameter] public CInputValidationStatus ValidationStatus { get; set; } = CInputValidationStatus.None;

        [Parameter] public RenderFragment? IconLeft { get; set; }
        [Parameter] public RenderFragment? IconRight { get; set; }
        [Parameter] public RenderFragment? PrefixContent { get; set; }
        [Parameter] public RenderFragment? SuffixContent { get; set; }

        [Parameter] public bool Clearable { get; set; }
        [Parameter] public bool ShowPasswordToggle { get; set; } = true;
        [Parameter] public bool IsLoading { get; set; }
        [Parameter] public bool Disabled { get; set; }
        [Parameter] public bool ReadOnly { get; set; }

        private ElementReference _inputElement;
        private bool _isFocused;
        private bool _showPassword;
        private string _inputType => (Type == "password" && _showPassword) ? "text" : Type;
        private bool _hasValue => !string.IsNullOrEmpty(CurrentValueAsString);

        private CInputValidationStatus EffectiveValidationStatus
        {
            get
            {
                if (ValidationStatus != CInputValidationStatus.None) return ValidationStatus;
                if (EditContext == null) return CInputValidationStatus.None;

                var messages = EditContext.GetValidationMessages(FieldIdentifier);
                if (messages.Any()) return CInputValidationStatus.Error;

                return CInputValidationStatus.None;
            }
        }

        private void HandleInput(ChangeEventArgs e)
        {
            CurrentValueAsString = e.Value?.ToString();
        }

        private void HandleFocus() => _isFocused = true;
        private void HandleBlur() => _isFocused = false;

        private async Task ClearValue()
        {
            CurrentValueAsString = string.Empty;
            await _inputElement.FocusAsync();
        }

        private void TogglePasswordVisibility()
        {
            _showPassword = !_showPassword;
        }

        private string GetContainerClasses()
        {
            var classes = new List<string>
        {
            $"lha-input-size-{Size.ToString().ToLower()}",
            (Disabled ? "is-disabled" : string.Empty),
            (ReadOnly ? "is-readonly" : string.Empty),
            (EditContext?.FieldCssClass(FieldIdentifier) ?? string.Empty)
        };
            return string.Join(" ", classes.Where(c => !string.IsNullOrEmpty(c)));
        }

        protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out TValue result,
            [NotNullWhen(false)] out string? validationErrorMessage)
        {
            if (typeof(TValue) == typeof(string))
            {
                result = (TValue)(object)(value ?? string.Empty);
                validationErrorMessage = null;
                return true;
            }
            else if (typeof(TValue) == typeof(int) || typeof(TValue) == typeof(int?))
            {
                if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedInt))
                {
                    result = (TValue)(object)parsedInt;
                    validationErrorMessage = null;
                    return true;
                }
                result = default!;
                validationErrorMessage = _localizer.L("Errors.InvalidNumberFormat");
                return false;
            }
            else if (typeof(TValue) == typeof(decimal) || typeof(TValue) == typeof(decimal?))
            {
                if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedDecimal))
                {
                    result = (TValue)(object)parsedDecimal;
                    validationErrorMessage = null;
                    return true;
                }
                result = default!;
                validationErrorMessage = _localizer.L("Errors.InvalidDecimalFormat");
                return false;
            }
            else if (typeof(TValue) == typeof(double) || typeof(TValue) == typeof(double?))
            {
                if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedDouble))
                {
                    result = (TValue)(object)parsedDouble;
                    validationErrorMessage = null;
                    return true;
                }
                result = default!;
                validationErrorMessage = _localizer.L("Errors.InvalidDoubleFormat");
                return false;
            }
            else if (typeof(TValue) == typeof(DateTime) || typeof(TValue) == typeof(DateTime?))
            {
                if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
                {
                    result = (TValue)(object)parsedDate;
                    validationErrorMessage = null;
                    return true;
                }
                result = default!;
                validationErrorMessage = _localizer.L("Errors.InvalidDateFormat");
                return false;
            }


            validationErrorMessage = null;
            try
            {
                if (BindConverter.TryConvertTo<TValue>(value, CultureInfo.CurrentCulture, out result))
                {
                    return true;
                }
                validationErrorMessage = _localizer.L("Errors.InvalidValue", value?.ToString() ?? string.Empty, typeof(TValue).Name);
                return false;
            }
            catch (Exception ex)
            {
                result = default!;
                validationErrorMessage = ex.Message;
                return false;
            }
        }
    }
}