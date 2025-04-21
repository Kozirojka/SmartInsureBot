# SmartInsureBot

**SmartInsureBot** is a Telegram bot designed to help users purchase insurance easily.

**Video of work placed near README.Md file**

## 🔧 Libraries Used

- `Telegram.Bot`
- `Mindee`
- `Microsoft.Extensions.Hosting`

## ⚙️ Technologies

- .NET 9

## 🧠 Workflow Explanation

I implemented the workflow using a **state machine** approach.

### How it works:

- The user is shown a menu with available commands.
- For example, they can send a command to **start the insurance purchase** process.
- The system tracks the current state of the user and, based on it, executes the appropriate scenario.
- Also there is workable Mindee api.

### 📁 Project Structure

- Inside the `Scenarios` folder, you will find different flows.
- For example, `CreateLicenseScenario` handles the license purchasing process.
- The logic uses a `switch` or similar construct to check the current user state and run the appropriate logic.


## ⚙️ AI-Driven Communication (Not Implemented Yet)
 
Unfortunately, I didn't manage to implement AI-driven communication in time.  
**But I do have an idea** on how it could work and improve the user experience.

The goal is to allow the user to send natural language messages, and the system would interpret the intent using OpenAI.  
Then, the result would be mapped to an internal bot command and routed to the correct scenario accordingly.

This concept could make interactions feel more intuitive and intelligent.

