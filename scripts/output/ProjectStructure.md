# Project Structure

_Generated automatically on 2026-03-16 17:09:38_

```
.
+-- .vscode
|   \-- settings.json
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
|           |   +-- _Imports.razor
|           |   +-- App.razor
|           |   +-- LHA.BlazorWasm.App.csproj
|           |   +-- MockAccessTokenProvider.cs
|           |   +-- Program.cs
|           |   \-- StatusBadgeModuleRegistration.cs
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
|           |   |   +-- CButtonIconPosition.cs
|           |   |   +-- CButtonSize.cs
|           |   |   +-- CButtonStyle.cs
|           |   |   \-- CButtonType.cs
|           |   +-- Emoji
|           |   |   +-- CEmojiCategory.cs
|           |   |   +-- EmojiCategoryBar.razor
|           |   |   +-- EmojiGrid.razor
|           |   |   +-- EmojiItem.razor
|           |   |   +-- EmojiModel.cs
|           |   |   +-- EmojiPicker.razor
|           |   |   +-- EmojiPicker.razor.cs
|           |   |   +-- EmojiPicker.razor.css
|           |   |   \-- EmojiSearch.razor
|           |   +-- Errors
|           |   |   +-- GlobalErrorBoundary.razor
|           |   |   +-- GlobalErrorBoundary.razor.cs
|           |   |   +-- GlobalErrorBoundary.razor.css
|           |   |   +-- LhaErrorBoundaryBase.cs
|           |   |   +-- NotFoundPage.razor
|           |   |   +-- NotFoundPage.razor.cs
|           |   |   \-- NotFoundPage.razor.css
|           |   +-- Form
|           |   |   +-- CFormFieldLayout.cs
|           |   |   +-- FormField.razor
|           |   |   +-- FormField.razor.cs
|           |   |   +-- FormField.razor.css
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
|           |   |   |   +-- CValidationStatus.cs
|           |   |   |   +-- DateRange.cs
|           |   |   |   +-- DateUtils.cs
|           |   |   |   +-- IPickerValueConverter.cs
|           |   |   |   +-- PickerBase.cs
|           |   |   |   +-- PickerPopup.razor
|           |   |   |   +-- PickerState.cs
|           |   |   |   +-- TimeView.razor
|           |   |   |   \-- ValidationMessage.razor
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
|           |   |       +-- CEditorCommand.cs
|           |   |       +-- EditorEnums.cs
|           |   |       +-- EditorOptions.cs
|           |   |       +-- EditorState.cs
|           |   |       \-- ToolbarConfig.cs
|           |   +-- Section
|           |   |   +-- CSectionVariant.cs
|           |   |   +-- Section.razor
|           |   |   +-- Section.razor.cs
|           |   |   \-- Section.razor.css
|           |   +-- Select
|           |   |   +-- CSelectMode.cs
|           |   |   +-- CSelectPlacement.cs
|           |   |   +-- Select.razor
|           |   |   +-- Select.razor.cs
|           |   |   +-- Select.razor.css
|           |   |   +-- Select.razor.js
|           |   |   +-- SelectItem.razor
|           |   |   +-- SelectItem.razor.cs
|           |   |   +-- SelectOption.cs
|           |   |   +-- SelectSearch.razor
|           |   |   +-- SelectState.cs
|           |   |   \-- SelectVirtualList.razor
|           |   +-- Sidebar
|           |   |   +-- Models
|           |   |   |   +-- CSidebarState.cs
|           |   |   |   \-- SidebarItemModel.cs
|           |   |   +-- Sidebar.razor
|           |   |   +-- Sidebar.razor.cs
|           |   |   +-- Sidebar.razor.css
|           |   |   +-- Sidebar.razor.js
|           |   |   +-- SidebarItem.razor
|           |   |   +-- SidebarItem.razor.cs
|           |   |   \-- SidebarItem.razor.css
|           |   +-- Skeleton
|           |   |   +-- CSkeletonAnimation.cs
|           |   |   +-- CSkeletonVariant.cs
|           |   |   +-- Skeleton.razor
|           |   |   +-- Skeleton.razor.cs
|           |   |   \-- Skeleton.razor.css
|           |   +-- Switch
|           |   |   +-- CSwitchLabelPosition.cs
|           |   |   +-- CSwitchSize.cs
|           |   |   +-- Switch.razor
|           |   |   +-- Switch.razor.cs
|           |   |   \-- Switch.razor.css
|           |   +-- ThemeSwitch
|           |   |   +-- CThemeSwitchVariant.cs
|           |   |   +-- ThemeSwitch.razor
|           |   |   +-- ThemeSwitch.razor.cs
|           |   |   \-- ThemeSwitch.razor.css
|           |   +-- Toast
|           |   |   +-- Toast.razor
|           |   |   +-- Toast.razor.cs
|           |   |   +-- Toast.razor.css
|           |   |   +-- ToastContainer.razor
|           |   |   +-- ToastContainer.razor.cs
|           |   |   \-- ToastContainer.razor.css
|           |   +-- Tooltip
|           |   |   +-- CTooltipPlacement.cs
|           |   |   +-- CTooltipTrigger.cs
|           |   |   +-- Tooltip.razor
|           |   |   +-- Tooltip.razor.cs
|           |   |   \-- Tooltip.razor.css
|           |   +-- _Imports.razor
|           |   +-- ComponentExtensions.cs
|           |   +-- LHA.BlazorWasm.Components.csproj
|           |   \-- LhaComponentBase.cs
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
|           |   |   +-- CLanguageCode.cs
|           |   |   +-- CLanguageSelectorMode.cs
|           |   |   +-- LanguageOption.cs
|           |   |   +-- LanguageProvider.cs
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
|           |   |   +-- CThemeMode.cs
|           |   |   +-- IThemeService.cs
|           |   |   +-- ThemeExtensions.cs
|           |   |   +-- ThemeService.cs
|           |   |   \-- ThemeState.cs
|           |   +-- Toast
|           |   |   +-- CToastLevel.cs
|           |   |   +-- IToastService.cs
|           |   |   +-- ToastExtensions.cs
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
