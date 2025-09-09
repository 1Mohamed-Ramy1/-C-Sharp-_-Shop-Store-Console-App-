# Project File: 🢃

[](https://www.notion.so/C-_-Shop-Store-Console-App-Final-Project-1e526769057680278f4edf1e4a9931b7?pvs=21)[https://www.notion.so/Shop-Store-Console-App-Final-Project-C-1e526769057680278f4edf1e4a9931b7?source=copy_link](https://www.notion.so/Shop-Store-Console-App-Final-Project-C-1e526769057680278f4edf1e4a9931b7?source=copy_link)

```Drive
**<https://drive.google.com/drive/folders/1SEnLHkQABzMcucF2MKVfCAfDkmyYszNw?usp=drive_link**>   
```

<aside> 🗣

### _**🎬 Presentation Summary**_

</aside>

> **Title: Shop Store Console App - User Login & Shopping System

Objective: Build a realistic, structured, object-oriented C# console application that allows users to register, log in, shop from a product list, manage a cart, and simulate purchases—all through a clean console interface. Admins have separate access to manage users and products.**

# _**🔢 Data Types & Structures**_

|Type|Purpose|
|---|---|
|`string`|Email, Password, Name|
|`int`|Quantity, Menu Choices|
|`double`|Prices|
|`bool`|Status flags|
|`List<T>`|For users, products, cart|
|`enum`|For user roles|
|`Dictionary<string, User>`|Optional optimization|
|`Stack/Queue`|Optional for undo/history|

## 🧪 **OOP Principles Used:**

|Principle|Application|
|---|---|
|Encapsulation|Data models and services hide internal logic|
|Inheritance|Shared interface or base class for pages|
|Polymorphism|Each page class overrides `Render()`|
|Abstraction|Services abstract business operations|

## 🧰 **Key Methods:**

```csharp
int GetValidInt(string prompt) {
    int number;
    while (!int.TryParse(Console.ReadLine(), out number)) {
        Console.WriteLine("Invalid number. Try again:");
    }
    return number;
}

```

### Cart Management:

```csharp
csharp
CopyEdit
public void AddProductToCart(Product product) { ... }
public void ViewCart() { ... }
public void RemoveItemFromCart(int index) { ... }
```

### Authentication:

```csharp
csharp
CopyEdit
public bool Login(string email, string password) {
    return users.Exists(u => u.Email == email && u.Password == password);
}

```

# _**📁 Project Folder Structure**_

### 📂 Pages:

Contains UI logic per screen:

- `HomePage`, `LoginPage`, `SignupPage`, `AdminPage`, `ShopPage`
- Each page handles its own input/output and invokes service logic

## 📂 Services:

Business logic and core operations:

- `AuthService`: Login, Sign-up, Admin validation
- `ProductService`: Load/search/add/delete/update products
- `CartService`: Add/remove/view/checkout items

# 📂 Data:

Manages reading/writing to local JSON files:

- `users.json`: Stores user credentials
- `products.json`: Stores product catalog

# 📂 Utils:

- General-purpose helpers:
    - Input validation
    - Display formatting
    - Password masking
    - Common reusable methods

# _**📦 Data Persistence with JSON**_

- `users.json` and `products.json` simulate a database
- Loaded at app start using `System.Text.Json`
- Written to when user/product data changes

```csharp

var json = File.ReadAllText("users.json");
var users = JsonSerializer.Deserialize<List<User>>(json);

```

Benefits:

- Persistent across sessions
- Editable, human-readable
- Lightweight storage mechanism for small apps

# _**🧱 Core Structure of the Code**_

```csharp
var router = new Router();

router.Register("home", () => new HomePage());
router.Register("login", () => new LoginPage());
router.Register("admin", () => new AdminPage());
router.Register("shop", () => new ShopPage());
router.Register("signup", () => new SignupPage());

router.Start("home");
```

- The `Router` acts like a navigation manager between virtual pages.
- Each page class implements a `Render()` method to handle its own logic.
- Simulates a web-app routing flow inside the console.

# _**⚙️ Services Breakdown**_

## 🔐 AuthService

- Methods: `Login()`, `Signup()`, `IsAdmin()`
- Handles credential validation and role-based logic

## 🛍️ ProductService

- Loads/saves product data from `products.json`
- Handles searching, adding, deleting, and updating products

## 🧺 CartService

- Manages cart items (`List<CartItem>`)
- Supports:
    - `AddToCart()`
    - `RemoveFromCart()`
    - `ViewCart()`
    - `Checkout()`

# _**📄 Pages**_

## 📌 **Namespace:**

```csharp
namespace App.Pages;
```

### 🧱 **Class Definition:**

```csharp
public abstract class Page
```

- An **abstract base class** for all pages in the application.
- Enforces a unified structure by requiring any subclass to implement:
    - `Display()` – for rendering the page's content.
    - `HandleInput(Router router)` – for managing user interactions and navigation.

---

### ✅ **Purpose:**

This class serves as the **foundation** for all pages in the application (e.g., `LoginPage`, `SignupPage`, `ShopPage`, `AdminPage`, etc.).

- Promotes consistency and modularity.
- Supports the **Router pattern** to manage page transitions.
- Simplifies expanding the app with new pages.

---

### 🧭 **Required Methods:**

|Method|Description|
|---|---|
|`void Display()`|Renders the page's interface and content (styled output, instructions, etc).|
|`void HandleInput(Router router)`|Processes user inputs, navigates using the `Router`.|

# _**🏠 Home Page**_

## 📂 **Namespace:**

```csharp
App.Pages
```

## 👤 **Inherits From:**

```csharp
Page (abstract base class)
```

---

## 🔍 **Purpose:**

The `HomePage` class represents the **landing screen** of the ELOSTORA console application. It introduces users to the platform with stylized ASCII art and branding, then guides them to either **Sign up**, **Log in**, or **Exit** the application.

---

## 📋 **Main Components:**

### 1. `Display()` Method

- Clears the console screen.
    
- Shows a stylized welcome banner using ASCII and `Print.OutLine()` with color.
    
- Displays a welcome message and tagline:
    
    `"✨ELOSTORA — Where Fashion Meets Elegance✨"`
    
- Prompts the user to press any key to continue.
    
- Uses Spectre.Console for markup enhancements (`AnsiConsole.MarkupLine`).
    

### 2. `HandleInput(Router router)` Method

- Displays a list of options using `Print.AskChoice()`:
    - 📝 Sign up
    - 🔐 Log in
    - ❌ Exit
- Based on user selection:
    - Navigates to **Sign up** page.
    - Navigates to **Log in** page.
    - Exits the app using the `Router`.

---

## ✅ **Features:**

- Fully styled UI with aligned color formatting.
- Encouraging, friendly introduction to the app.
- Keyboard-driven input handling with clear prompts.
- Supports the router system for clean navigation.

# _**🧾 Sign up Page**_

## 🎯 **Purpose**:

The `SignupPage` class handles the user registration process, guiding new users through creating an account with proper validation checks like username, email, and password.

---

## 📋 **Main Features**:

### **Display and Input Handling**:

- **Display**: Shows a header prompting users to create an account.
- **User Registration**:
    - **Username**: Prompts the user for a username. Checks if the input is empty.
    - **Email**: Ensures the email ends with `@gmail.com`. If not, prompts the user to correct it.
    - **Email Uniqueness**: Checks if the email already exists in the database. If it does, the user is given the option to retry the signup process.
    - **Password**: Prompts the user for a password (hidden input for security).

### **Account Creation**:

- If the email does not already exist:
    - Creates a new `User` instance and adds it to the database.
    - Shows a success message upon successful account creation.

### **Post-Signup Options**:

- **Login Prompt**: After account creation, users are asked if they want to log in immediately.
    - If yes, the app navigates to the login page.
    - If no, the app navigates back to the home page.

### **Back Option**:

- Users can return to the home page anytime by selecting the "Back" option.

---

## ⌨️ **User Interaction**:

- Uses `Print.AskChoice()` for navigating the signup process.
- Prompts for username and email with validation checks.
- Utilizes `AnsiConsole.Prompt()` for password input, ensuring it’s hidden.
- Uses `Print.ErrorMsg()` to display error messages for invalid inputs.
- Allows users to retry signup if email is already taken or if errors occur.

---

## 🧠 **Utilities Used**:

- `DataManger.UserDB`: Access to the user database for adding new users and checking email uniqueness.
- `Print`: For styled messages like error notifications and success confirmations.
- `Router.Navigate()`: Used to navigate between different pages (home, login).

# _**🧾 `Login Page`**_

## 🎯 **Purpose:**

The `LoginPage` class represents the login screen where users (both regular and admin) can authenticate. It manages login input, handles incorrect credentials, and routes users based on their role.

## **Inherits from:**

`Page` (abstract base class)

---

## 🔧 Core Responsibilities

### 1. **Display()**

- Clears the console and prints a title using `Spectre.Console` and `Print`.
- Displays a styled heading: **🔐 LOGIN TO YOUR ACCOUNT**.

### 2. **HandleInput(Router router)**

- Runs a login loop allowing:
    - 📧 **Email input**:
        - If a user already logged in during the session, it suggests the **last used email** (non-admins only).
    - 🔑 **Password input**:
        - Uses `TextPrompt` with `Secret()` from `Spectre.Console` to hide input.

---

## 🔐 Authentication Logic

### ✅ Admin Login:

- Checked using `AdminManager.IsAdmin(email, pass)`.
- If matched:
    - Saves the admin user to `GlobalStore.Instance.CurrentUser`.
        
    - Displays a success message.
        
    - Asks:
        
        → Go to Admin Page
        
        → Go to Shop Page as Admin
        
    - Navigates accordingly.
        

### ✅ Regular User Login:

- Fetches all users from `DataManger.UserDB`.
- Checks for matching email (case-insensitive) and exact password.
- If found:
    - Saves user to `GlobalStore.Instance.CurrentUser`.
    - Welcomes the user with their username.
    - If not admin → saves their email as `LastUsedEmail` (for session recall).
    - Routes:
        - If admin: asks if they want the Admin Page.
        - If not: directly goes to Shop Page.

## ❌ Invalid Credentials:

- Displays an error message: "❌ Invalid Email or Password!"
- Asks if the user wants to try again.
    - If yes → restarts input.
    - If no → navigates back to the Home Page.

---

## 🧠 Session Memory Feature

- Temporary session memory stores **non-admin emails** for user convenience.
- Reset when app restarts.

---

## 🛠 Dependencies Used

- `App.Models`, `App.Routes`, `App.Services`, `App.Utils`
- `Spectre.Console` for styled console prompts and inputs.

# _**🧑‍💻 Admin Login page**_

## 🎯 **Purpose**:

The `AdminPage` class serves as the main admin control panel, allowing administrators to manage users and products through an interactive and colorful console interface using `Spectre.Console`.

---

## 📋 **Main Features**:

### 🧑‍💻 **User Management**:

- **👥 View Users**: Lists all users with ID, username, email, and warning status.
- **➕ Add User**: Prompts admin to input username, email, and password to create a new user.
- **❌ Delete User**: Allows deletion of a user by selecting their ID.
- **⚠️ Warn User**: Marks a user as warned (adds a warning flag).

## 📦 **Product Management**:

- **➕ Add Product**: Lets admin input product name, category (preset or custom), price, and stock per size (S, M, L, etc.).
- **✏️ Edit Product**: Enables editing product details (name, category, price, stock by size).
- **🗑️ Delete Product**: Deletes a product by its ID.
- **🧾 View Products**: Displays all products in a formatted table including sizes and stock.

### 🚪 **Logout**:

- Asks for confirmation before logging out and returning to the home page.

---

## ⌨️ **User Interaction**:

- Uses `Print.AskChoice()` for selecting actions.
- Input fields support [Esc] to cancel via `AskWithEsc()` method.
- Invalid entries are caught with clear error messages.
- All major actions (Add/Edit/Delete) confirm success or failure with styled feedback.
- Tables are styled using Spectre.Console's `Table` with colors and emojis for clarity.

---

## 🧠 **Utilities Used**:

- `GlobalStore` for current user.
- `DataManger` for accessing user and product databases.
- `Print` utility class for styled output, input prompts, and pauses.

Application starts at `HomePage`

- Users can either **Sign up** or **Log in**
    - Sign-up collects and stores user data in `users.json`
    - Login verifies credentials and routes user based on role

Email:

```
admin
```

Password:

```
admin
```

Grants access to Admin Dashboard for product and user management.

Based on role:

- Admins → `AdminPage`
- Users → `ShopPage`

# 🔧 Admin Page Functionalities

- View all users
- View, add, delete, or update products
- Role-based access control to restrict features to admins only

# _**🛒 Shop Page & Cart System**_

## ✅ **Main Structure:**

### General Structure:

- `ShopPage` inherits from `Page`.
- Maintains a private cart `Dictionary<int, (ProductSize Size, int Quantity)>`.

### 📺 **UI Display:**

- Displays a stylish welcome message with username.
    
- Uses Spectre.Console to format options and output with color and design.
    
- Inherits from `Page`.
    
- Contains a `Cart` dictionary to store selected products in the form:
    
    `ProductId → (Size, Quantity)`
    

## Key Methods:

- `Display()`: Displays a welcome message.
- `HandleInput(Router router)`: Controls the main shop menu and handles user input.

### 📌 **Main Options (HandleInput):**

1. **📁 Browse Products** → `ListProducts()`
2. **🛒 View Cart** → `ViewCart()`
3. **📜 View History** → `ViewHistory()`
4. **💳 Checkout** → `Checkout()`
5. **🚪 Log Out** → Confirms with Y/N before navigating to `home`.

### 🧾 **ListProducts():**

- Retrieves and sorts products by category, then name, then ID.
- Displays a colored product table with:
    - **ID / Name / Category / Price / Available Sizes**
    - Sizes are shown with remaining quantity.
    - Sizes with 0 quantity are shown in red.
- Uses `AddToCart()` to let users select product by ID, size, and quantity.
- Supports repeated additions with confirmation prompt.

### ➕ **AddToCart():**

- Accepts:
    - **Product ID** (with cancel support)
    - **Size** (selected from enum `SizeUnit`)
    - **Quantity**
- Validates product, size, and stock.
- If valid:
    - Adds to cart (merges with existing entry if same product+size exists).
    - Updates product quantity in DB.
    - Plays **beep sound** after successful addition.
    - Displays confirmation banner.
- Handles invalid input and out-of-stock gracefully with sound and error message.

### 🛒 **ViewCart():**

- Displays cart items with:
    - Product name
    - Size
    - Quantity
    - Price (total for quantity)
- Shows **total amount due** at bottom.
- If cart is empty, displays a warning.

### 🧾 **ViewHistory():**

- Gets orders for current user.
- Shows table with:
    - Order ID
    - Date
    - Total amount
- If no orders, informs user.

# 💳 Checkout():

- Verifies if the cart is empty before continuing.
- Displays available payment methods:
    - **PayPal** → Balance, Credit, or Bank Account (auto deducts).
    - **Visa** → Auto deduction.
    - **Cash On Delivery** → Prompts user to input amount manually.
- Confirms the total, processes payment, and:
    - Saves order to `History.txt`.
    - Plays a success sound.
    - Clears the cart.

# _**🎯 Extra Features**_

- ESC key message displayed when browsing.
- Stylish color formatting using Spectre.Console.
- Input prompts are clear, step-by-step, and validated.
- Sound feedback for success and errors (Windows only).
- Product sizes shown with remaining stock after current cart is considered.
- ESC key message displayed when browsing.
- Stylish color formatting using Spectre.Console.
- Input prompts are clear, step-by-step, and validated.
- Sound feedback for success and errors (Windows only).
- Product sizes shown with remaining stock after current cart is considered.

# _**Conclusion**_

> This project, built during my first year in Computer Science 🎓, stands as a testament to both my dedication and the skills I’ve acquired as a student. By designing and implementing the **ELOSTORA_Console**, a fully functional console-based e-commerce system 💻, I was able to integrate a variety of key features, such as dynamic user and admin functionalities 👤👑, advanced product management 🛍️, and enhanced payment options 💳. The system's flexibility, user-friendly interface, and seamless experience reflect my growing proficiency in object-oriented programming 💻, database management 🗂️, and UI/UX design 🎨.

> Through the creation of this project, I not only deepened my understanding of **C#** and problem-solving 🔍 but also honed my ability to create practical, user-centered applications 🛠️. I successfully integrated critical features like user authentication 🔐, product management 📦, and a dynamic admin panel 🖥️, all while ensuring a high-quality user experience 🌟.

> This project has reinforced my enthusiasm for building innovative software that serves both practical and creative purposes 💡. It has empowered me with a clearer vision of how theoretical knowledge translates into real-world applications 🌐 and further fueled my passion for software development 💻🔥. I’m excited to continue building on this foundation, exploring new technologies 🛠️, and tackling even more complex challenges in the future 🚀.

> This is just the beginning, and I look forward to the continued journey of applying what I’ve learned to future opportunities and challenges 🎯.
