# Electron.Clipboard

Perform copy and paste operations on the system clipboard.

## Overview

The `Electron.Clipboard` API provides comprehensive access to the system clipboard, supporting multiple data formats including text, HTML, RTF, images, and custom data. It enables reading from and writing to the clipboard with platform-specific behavior.

## Methods

#### 🧊 `Task<string[]> AvailableFormatsAsync(string type = "")`
Get an array of supported formats for the clipboard type.

**Parameters:**
- `type` - Clipboard type

**Returns:**

An array of supported formats for the clipboard type.

#### 🧊 `void Clear(string type = "")`
Clears the clipboard content.

**Parameters:**
- `type` - Clipboard type

#### 🧊 `Task<ReadBookmark> ReadBookmarkAsync()`
Returns an Object containing title and url keys representing the bookmark in the clipboard. The title and url values will be empty strings when the bookmark is unavailable.

**Returns:**

Object containing title and url keys representing the bookmark in the clipboard.

#### 🧊 `Task<string> ReadFindTextAsync()`
macOS: The text on the find pasteboard. This method uses synchronous IPC when called from the renderer process. The cached value is reread from the find pasteboard whenever the application is activated.

**Returns:**

The text on the find pasteboard.

#### 🧊 `Task<string> ReadHTMLAsync(string type = "")`
Read the content in the clipboard as HTML markup.

**Parameters:**
- `type` - Clipboard type

**Returns:**

The content in the clipboard as markup.

#### 🧊 `Task<NativeImage> ReadImageAsync(string type = "")`
Read an image from the clipboard.

**Parameters:**
- `type` - Clipboard type

**Returns:**

An image from the clipboard.

#### 🧊 `Task<string> ReadRTFAsync(string type = "")`
Read the content in the clipboard as RTF.

**Parameters:**
- `type` - Clipboard type

**Returns:**

The content in the clipboard as RTF.

#### 🧊 `Task<string> ReadTextAsync(string type = "")`
Read the content in the clipboard as plain text.

**Parameters:**
- `type` - Clipboard type

**Returns:**

The content in the clipboard as plain text.

#### 🧊 `void Write(Data data, string type = "")`
Writes data to the clipboard.

**Parameters:**
- `data` - Data object to write
- `type` - Clipboard type

#### 🧊 `void WriteBookmark(string title, string url, string type = "")`
Writes the title and url into the clipboard as a bookmark.

Note: Most apps on Windows don't support pasting bookmarks into them so you can use clipboard.write to write both a bookmark and fallback text to the clipboard.

**Parameters:**
- `title` - Bookmark title
- `url` - Bookmark URL
- `type` - Clipboard type

#### 🧊 `void WriteFindText(string text)`
macOS: Writes the text into the find pasteboard as plain text. This method uses synchronous IPC when called from the renderer process.

**Parameters:**
- `text` - Text to write to find pasteboard

#### 🧊 `void WriteHTML(string markup, string type = "")`
Writes markup to the clipboard.

**Parameters:**
- `markup` - HTML markup to write
- `type` - Clipboard type

#### 🧊 `void WriteImage(NativeImage image, string type = "")`
Writes an image to the clipboard.

**Parameters:**
- `image` - Image to write to clipboard
- `type` - Clipboard type

#### 🧊 `void WriteRTF(string text, string type = "")`
Writes the text into the clipboard in RTF.

**Parameters:**
- `text` - RTF content to write
- `type` - Clipboard type

#### 🧊 `void WriteText(string text, string type = "")`
Writes the text into the clipboard as plain text.

**Parameters:**
- `text` - Text content to write
- `type` - Clipboard type

## Usage Examples

### Basic Text Operations

```csharp
// Read text from clipboard
var text = await Electron.Clipboard.ReadTextAsync();
Console.WriteLine($"Clipboard text: {text}");

// Write text to clipboard
Electron.Clipboard.WriteText("Hello, Electron.NET!");

// Read with specific type
var html = await Electron.Clipboard.ReadHTMLAsync("public.main");
```

### Rich Content Handling

```csharp
// Copy formatted text
var htmlContent = "<h1>Title</h1><p>Some <strong>bold</strong> text</p>";
Electron.Clipboard.WriteHTML(htmlContent);

// Read RTF content
var rtf = await Electron.Clipboard.ReadRTFAsync();
Console.WriteLine($"RTF content: {rtf}");
```

### Image Operations

```csharp
// Read image from clipboard
var image = await Electron.Clipboard.ReadImageAsync();
if (image != null)
{
    Console.WriteLine($"Image size: {image.Size.Width}x{image.Size.Height}");
}

// Write image to clipboard
var nativeImage = NativeImage.CreateFromPath("screenshot.png");
Electron.Clipboard.WriteImage(nativeImage);
```

### Bookmark Management

```csharp
// Read bookmark from clipboard
var bookmark = await Electron.Clipboard.ReadBookmarkAsync();
if (!string.IsNullOrEmpty(bookmark.Title))
{
    Console.WriteLine($"Bookmark: {bookmark.Title} -> {bookmark.Url}");
}

// Write bookmark to clipboard
Electron.Clipboard.WriteBookmark("Electron.NET", "https://github.com/ElectronNET/Electron.NET");
```

### Advanced Clipboard Operations

```csharp
// Check available formats
var formats = await Electron.Clipboard.AvailableFormatsAsync();
Console.WriteLine($"Available formats: {string.Join(", ", formats)}");

// Clear clipboard
Electron.Clipboard.Clear();

// Write custom data
var data = new Data
{
    Text = "Custom data",
    Html = "<p>Custom HTML</p>",
    Image = nativeImage
};
Electron.Clipboard.Write(data);
```

### macOS Find Pasteboard

```csharp
// macOS specific find pasteboard operations
if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
{
    // Read find text
    var findText = await Electron.Clipboard.ReadFindTextAsync();
    Console.WriteLine($"Find text: {findText}");

    // Write find text
    Electron.Clipboard.WriteFindText("search term");
}
```

## Related APIs

- [Electron.Shell](Shell.md) - Work with file paths from clipboard
- [Electron.Notification](Notification.md) - Show clipboard operation results

## Additional Resources

- [Electron Clipboard Documentation](https://electronjs.org/docs/api/clipboard) - Official Electron clipboard API
