# Project Structure

_Generated automatically on 2026-03-15 23:08:43_

```
.
+-- .github
|   \-- workflows
+-- .vscode
|   \-- settings.json
+-- deploy
|   \-- charts
|       \-- lha-webui
|           \-- templates
+-- src
|   \-- WebUI
|       \-- blazorwasm
|           +-- LHA.BlazorWasm.App
|           |   +-- Layout
|           |   |   +-- MainLayout.razor
|           |   |   +-- MainLayout.razor.css
|           |   |   +-- NavMenu.razor
|           |   |   \-- NavMenu.razor.css
|           |   +-- Pages
|           |   |   +-- Counter.razor
|           |   |   +-- EditorExample.razor
|           |   |   +-- Home.razor
|           |   |   +-- NotFound.razor
|           |   |   +-- Test.razor
|           |   |   \-- Weather.razor
|           |   +-- Properties
|           |   |   \-- launchSettings.json
|           |   +-- App.razor
|           |   +-- LHA.BlazorWasm.App.csproj
|           |   +-- MockAccessTokenProvider.cs
|           |   +-- Program.cs
|           |   +-- StatusBadgeModuleRegistration.cs
|           |   \-- _Imports.razor
|           +-- LHA.BlazorWasm.Components
|           |   +-- Badges
|           |   |   +-- StatusBadge.razor
|           |   |   +-- StatusBadge.razor.cs
|           |   |   \-- StatusBadge.razor.css
|           |   +-- Breadcrumb
|           |   |   +-- Breadcrumb.razor
|           |   |   +-- Breadcrumb.razor.cs
|           |   |   +-- Breadcrumb.razor.css
|           |   |   +-- BreadcrumbItem.razor
|           |   |   +-- BreadcrumbItem.razor.cs
|           |   |   \-- BreadcrumbItemModel.cs
|           |   +-- Buttons
|           |   |   +-- Button.razor
|           |   |   +-- Button.razor.cs
|           |   |   +-- Button.razor.css
|           |   |   +-- ButtonIconPosition.cs
|           |   |   +-- ButtonSize.cs
|           |   |   +-- ButtonStyle.cs
|           |   |   \-- ButtonType.cs
|           |   +-- Emoji
|           |   |   +-- EmojiCategory.cs
|           |   |   +-- EmojiCategoryBar.razor
|           |   |   +-- EmojiGrid.razor
|           |   |   +-- EmojiItem.razor
|           |   |   +-- EmojiModel.cs
|           |   |   +-- EmojiPicker.razor
|           |   |   +-- EmojiPicker.razor.cs
|           |   |   +-- EmojiPicker.razor.css
|           |   |   \-- EmojiSearch.razor
|           |   +-- Errors
|           |   |   +-- GlobalException
|           |   |   +-- NotFound
|           |   |   +-- GlobalErrorBoundary.razor
|           |   |   +-- GlobalErrorBoundary.razor.cs
|           |   |   +-- GlobalErrorBoundary.razor.css
|           |   |   +-- LhaErrorBoundaryBase.cs
|           |   |   +-- NotFoundPage.razor
|           |   |   +-- NotFoundPage.razor.cs
|           |   |   \-- NotFoundPage.razor.css
|           |   +-- Form
|           |   |   +-- Internal
|           |   |   +-- FormField.razor
|           |   |   +-- FormField.razor.cs
|           |   |   +-- FormField.razor.css
|           |   |   +-- FormFieldLayout.cs
|           |   |   +-- FormHelp.razor
|           |   |   +-- FormLabel.razor
|           |   |   \-- FormMessage.razor
|           |   +-- LanguageSelector
|           |   |   +-- LanguageSelector.razor
|           |   |   +-- LanguageSelector.razor.cs
|           |   |   +-- LanguageSelector.razor.css
|           |   |   \-- LanguageSelector.razor.js
|           |   +-- Pickers
|           |   |   +-- Core
|           |   |   |   +-- CalendarView.razor
|           |   |   |   +-- DateRange.cs
|           |   |   |   +-- DateUtils.cs
|           |   |   |   +-- IPickerValueConverter.cs
|           |   |   |   +-- PickerBase.cs
|           |   |   |   +-- PickerPopup.razor
|           |   |   |   +-- PickerState.cs
|           |   |   |   +-- TimeView.razor
|           |   |   |   +-- ValidationMessage.razor
|           |   |   |   \-- ValidationStatus.cs
|           |   |   +-- DatePicker
|           |   |   |   +-- DatePicker.razor
|           |   |   |   +-- DatePicker.razor.cs
|           |   |   |   \-- DatePicker.razor.css
|           |   |   +-- DateRangePicker
|           |   |   |   +-- DateRangePicker.razor
|           |   |   |   +-- DateRangePicker.razor.cs
|           |   |   |   \-- DateRangePicker.razor.css
|           |   |   +-- DateTimePicker
|           |   |   |   +-- DateTimePicker.razor
|           |   |   |   +-- DateTimePicker.razor.cs
|           |   |   |   \-- DateTimePicker.razor.css
|           |   |   +-- DateTimeRangePicker
|           |   |   |   +-- DateTimeRangePicker.razor
|           |   |   |   +-- DateTimeRangePicker.razor.cs
|           |   |   |   \-- DateTimeRangePicker.razor.css
|           |   |   +-- TimePicker
|           |   |   |   +-- TimePicker.razor
|           |   |   |   +-- TimePicker.razor.cs
|           |   |   |   \-- TimePicker.razor.css
|           |   |   \-- TimeRangePicker
|           |   |       +-- TimeRangePicker.razor
|           |   |       +-- TimeRangePicker.razor.cs
|           |   |       \-- TimeRangePicker.razor.css
|           |   +-- RichTextEditor
|           |   |   +-- Components
|           |   |   |   +-- CodeBlockDialog.razor
|           |   |   |   +-- CodeBlockDialog.razor.cs
|           |   |   |   +-- ColorPickerPopup.razor
|           |   |   |   +-- ColorPickerPopup.razor.cs
|           |   |   |   +-- DragDropDialog.razor
|           |   |   |   +-- DragDropDialog.razor.cs
|           |   |   |   +-- EditorContent.razor
|           |   |   |   +-- EditorStatusBar.razor
|           |   |   |   +-- EditorToolbar.razor
|           |   |   |   +-- EditorToolbar.razor.cs
|           |   |   |   +-- Icons.cs
|           |   |   |   +-- ImageDialog.razor
|           |   |   |   +-- ImageDialog.razor.cs
|           |   |   |   +-- LinkDialog.razor
|           |   |   |   +-- LinkDialog.razor.cs
|           |   |   |   +-- RichTextEditor.razor
|           |   |   |   +-- RichTextEditor.razor.cs
|           |   |   |   +-- SpecialCharsDialog.razor
|           |   |   |   +-- SpecialCharsDialog.razor.cs
|           |   |   |   +-- SpecialCharsPopup.razor
|           |   |   |   +-- SpecialCharsPopup.razor.cs
|           |   |   |   +-- TableDialog.razor
|           |   |   |   +-- TableDialog.razor.cs
|           |   |   |   +-- ToolbarButton.razor
|           |   |   |   +-- ToolbarButton.razor.cs
|           |   |   |   +-- ToolbarDropdown.razor
|           |   |   |   +-- ToolbarDropdown.razor.cs
|           |   |   |   \-- ToolbarSeparator.razor
|           |   |   +-- Interop
|           |   |   |   \-- RichTextEditorInterop.cs
|           |   |   \-- Models
|           |   |       +-- EditorCommand.cs
|           |   |       +-- EditorOptions.cs
|           |   |       +-- EditorState.cs
|           |   |       \-- ToolbarConfig.cs
|           |   +-- Section
|           |   |   +-- Section.razor
|           |   |   +-- Section.razor.cs
|           |   |   +-- Section.razor.css
|           |   |   \-- SectionVariant.cs
|           |   +-- Select
|           |   |   +-- Select.razor
|           |   |   +-- Select.razor.cs
|           |   |   +-- Select.razor.css
|           |   |   +-- Select.razor.js
|           |   |   +-- SelectItem.razor
|           |   |   +-- SelectItem.razor.cs
|           |   |   +-- SelectMode.cs
|           |   |   +-- SelectOption.cs
|           |   |   +-- SelectPlacement.cs
|           |   |   +-- SelectSearch.razor
|           |   |   +-- SelectState.cs
|           |   |   \-- SelectVirtualList.razor
|           |   +-- Skeleton
|           |   |   +-- Skeleton.razor
|           |   |   +-- Skeleton.razor.cs
|           |   |   +-- Skeleton.razor.css
|           |   |   +-- SkeletonAnimation.cs
|           |   |   \-- SkeletonVariant.cs
|           |   +-- Switch
|           |   |   +-- Switch.razor
|           |   |   +-- Switch.razor.cs
|           |   |   +-- Switch.razor.css
|           |   |   +-- SwitchLabelPosition.cs
|           |   |   \-- SwitchSize.cs
|           |   +-- ThemeSwitch
|           |   |   +-- ThemeSwitch.razor
|           |   |   +-- ThemeSwitch.razor.cs
|           |   |   +-- ThemeSwitch.razor.css
|           |   |   \-- ThemeSwitchVariant.cs
|           |   +-- Toast
|           |   |   +-- Toast.razor
|           |   |   +-- Toast.razor.cs
|           |   |   +-- Toast.razor.css
|           |   |   +-- ToastContainer.razor
|           |   |   +-- ToastContainer.razor.cs
|           |   |   \-- ToastContainer.razor.css
|           |   +-- Tooltip
|           |   |   +-- Tooltip.razor
|           |   |   +-- Tooltip.razor.cs
|           |   |   +-- Tooltip.razor.css
|           |   |   +-- TooltipPlacement.cs
|           |   |   \-- TooltipTrigger.cs
|           |   +-- ComponentExtensions.cs
|           |   +-- LHA.BlazorWasm.Components.csproj
|           |   +-- LhaComponentBase.cs
|           |   \-- _Imports.razor
|           +-- LHA.BlazorWasm.HttpApi.Client
|           |   +-- Abstractions
|           |   |   +-- IAccessTokenProvider.cs
|           |   |   +-- IApiClient.cs
|           |   |   +-- IApiErrorHandler.cs
|           |   |   \-- IClientContextProvider.cs
|           |   +-- Clients
|           |   |   \-- ExampleApiClient.cs
|           |   +-- Core
|           |   |   +-- ApiClientBase.cs
|           |   |   +-- ApiError.cs
|           |   |   +-- ApiException.cs
|           |   |   +-- ApiResponse.cs
|           |   |   +-- DefaultApiErrorHandler.cs
|           |   |   \-- DefaultClientContextProvider.cs
|           |   +-- Extensions
|           |   |   \-- HttpApiClientExtensions.cs
|           |   +-- Handlers
|           |   |   +-- AuthMessageHandler.cs
|           |   |   +-- ContextMessageHandler.cs
|           |   |   +-- LoggingMessageHandler.cs
|           |   |   +-- RetryMessageHandler.cs
|           |   |   \-- SecureHttpHandler.cs
|           |   +-- Options
|           |   |   \-- HttpApiClientOptions.cs
|           |   +-- Serialization
|           |   |   \-- JsonOptionsProvider.cs
|           |   \-- LHA.BlazorWasm.HttpApi.Client.csproj
|           +-- LHA.BlazorWasm.Modules
|           |   \-- LHA.BlazorWasm.Modules.csproj
|           +-- LHA.BlazorWasm.Services
|           |   +-- ErrorHandling
|           |   |   +-- ErrorReporter.cs
|           |   |   +-- ErrorReportingExtensions.cs
|           |   |   +-- IErrorReporter.cs
|           |   |   \-- ToastApiErrorHandler.cs
|           |   +-- Localization
|           |   |   +-- LanguageCode.cs
|           |   |   +-- LanguageOption.cs
|           |   |   +-- LanguageProvider.cs
|           |   |   +-- LanguageSelectorMode.cs
|           |   |   +-- LocalizationExtensions.cs
|           |   |   +-- LocalizationOptions.cs
|           |   |   \-- LocalizationService.cs
|           |   +-- StatusBadge
|           |   |   +-- IStatusBadgeService.cs
|           |   |   +-- StatusBadgeConfiguration.cs
|           |   |   \-- StatusBadgeService.cs
|           |   +-- Storage
|           |   |   +-- ILocalStorageService.cs
|           |   |   +-- LocalStorageService.cs
|           |   |   +-- StorageExtensions.cs
|           |   |   \-- StorageOptions.cs
|           |   +-- Theme
|           |   |   +-- IThemeService.cs
|           |   |   +-- ThemeExtensions.cs
|           |   |   +-- ThemeMode.cs
|           |   |   +-- ThemeService.cs
|           |   |   \-- ThemeState.cs
|           |   +-- Toast
|           |   |   +-- IToastService.cs
|           |   |   +-- ToastExtensions.cs
|           |   |   +-- ToastLevel.cs
|           |   |   +-- ToastMessage.cs
|           |   |   +-- ToastService.cs
|           |   |   \-- ToastState.cs
|           |   \-- LHA.BlazorWasm.Services.csproj
|           +-- LHA.BlazorWasm.Shared
|           |   +-- Abstractions
|           |   |   \-- Localization
|           |   |       \-- ILocalizationService.cs
|           |   +-- Constants
|           |   |   +-- Formatters
|           |   |   |   \-- DateTimeFormatter.cs
|           |   |   \-- CustomHttpHeaderNames.cs
|           |   +-- Localization
|           |   |   +-- en.json
|           |   |   \-- vi.json
|           |   +-- Models
|           |   |   +-- Localization
|           |   |   |   \-- LocalizationState.cs
|           |   |   +-- StatusBadge
|           |   |   |   \-- StatusBadgeModels.cs
|           |   |   \-- ExampleEnums.cs
|           |   \-- LHA.BlazorWasm.Shared.csproj
|           \-- LHA.Security
|               +-- Device
|               |   \-- DeviceFingerprintService.cs
|               +-- Encryption
|               |   +-- AesEncryptionService.cs
|               |   \-- RsaEncryptionService.cs
|               +-- Keys
|               |   \-- KeyRotationService.cs
|               +-- Middleware
|               |   \-- SecureRequestMiddleware.cs
|               +-- Options
|               |   \-- SecurityOptions.cs
|               +-- ReplayProtection
|               |   \-- ReplayProtectionService.cs
|               +-- Signing
|               |   \-- RequestSigner.cs
|               \-- LHA.Security.csproj
+-- test
|   \-- Test.API
|       +-- appsettings.Development.json
|       +-- appsettings.json
|       +-- Program.cs
|       +-- Test.API.csproj
|       \-- Test.API.http
+-- .dockerignore
+-- .gitignore
+-- Dockerfile
+-- LHA.WebUI.slnx
+-- nginx.conf
\-- render.yaml
```
