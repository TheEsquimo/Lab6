# Lab6

# To-Do-List
### WPF ELEMENTS
* Label to display time left till bar closes
  * This isn't the end of the simulation, just the time till the bar doesn't let any new guests in
  * The bar actually closes (the simulation ends) when all guests have gone home
* Label to display number of guests in the bar
* Label to display number of glasses on the shelf
* Label to display number of available/free seats/chairs
* ListBox for bartender
* ListBox for waiter
* ListBox for bouncer + guests
* When something happens an agent, make it readable in the corresponding ListBox
  * Use Items.Insert to add text-messages, as opposed to Items.Add
  * At the start of the text-message, there should be an "order number" or timestamp.
  * The specific messages that should be displayed can be found under it's corresponding agent to-do-section
* Add pause/continue buttons under each ListBox, tied to the corresponding processess //Optional, but helpful
* Add "panic button" which stops all threads //Optional, but helpful

### Shelf and Chairs
* To represent the shelf and chairs; use threadsafe collection such as
  * BlockingCollection
  * ConcurrentQueue<T>
  
## Agents
> Sentences marked with **bold** are actions that should be displayed to the user through text
### Bartender
* Has it's own thread
* **Waits in the bar** for customer to show up
* As soon as a customer arrives, **The bartender goes to the shelf** to pick up a glass
 * This action should take three seconds
* Then he **Pours a glass of beer for the customer** and waits for the next customer again
 * This action should take three seconds
* **Bartender goes home** when all the last customers have left

### Waiter
* **Picks up empty glasses** that exist on the tables
 * This action takes ten seconds
* Then she **Washes glasses**
 * This action takes fifteen seconds
* Then she **Puts the glasses on the shelf**
* **Waiter goes home** when all the last customers have left

###



