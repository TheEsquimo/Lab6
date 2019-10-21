# Lab6

# To-Do-List
### WPF ELEMENTS
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
* ShelfList: 8 glasses
* ChairsList: 9 chairs
* EmptyGlassesList
* WaitressDishesList
* GuestsWaitingForBeerQueue
* GuestsWaitingForSeatQueue
  
## Agents
> Sentences marked with **bold** are actions that should be displayed to the user through text

> Whenever the time of actions are given, they should technically be theSpeedGiven * speedMultiplier
### Bartender
* Has it's own thread
* **Waits in the bar** for customer to show up
* As soon as a customer arrives, **The bartender goes to the shelf** to pick up a glass
 * This action should take three seconds
* Then he **Pours a glass of beer for the customer** and waits for the next customer again
 * This action should take three seconds
* **Bartender goes home** when all the last customers have left

### Waiter
* GlassesList for dishes to be washed and then returned to the shelf

ROUTINE:
* **Picks up empty glasses** that exist on the tables
 * This action takes ten seconds
* Then she **Washes glasses**
 * This action takes fifteen seconds
* Then she **Puts the glasses on the shelf**
* **Waiter goes home** when all the last customers have left

### Bouncer
ROUTINE:
* Lets in customers at random time-intervals
 * Three to ten seconds
* Checks ID, basically giving the customer a random name from a list of predefined ones
* The bouncer stops letting new customers in when the bar closes and **The bounces goes home**

### Patron
string name
Glass glass

ROUTINE:
* **Customer enter pub** and goes directly to the bar
 * This action takes one second
* Waits until the bartender gives beer
* Then **Customer looks for an empty seat**
 * Getting to the seat takes four seconds
 * If there are no seats available, they wait until there is one
* **Customer sits down** and drinks their beer
 * Finishing the beer takes between ten and twenty seconds (randomize)
* When they are done with their beer, the **Customer leaves**

## Other classes
### Chair
Guest currentSitter

### Glass


## Testing
* Test the application with different values of parameters
 * Standard values
 * 20 glasses, 3 chairs
 * 5 glasses, 20 chairs
 * Guests stays double the amount of time
 * The waiter picks up and does the dishes at twice the speed
 * The bar is open for five minutes or longer
 * Couples night - bartender always lets in two customers whenever they let someone in
 * Double the amount of time between bouncer let-ins, after 20 seconds 15 guests are let in at once (one-time event)
 
# Bedömning
När ni är klara med laborationen ska ni se till att en annan grupp granskar er laboration. Det är ert eget ansvar att se till att just er laboration blir granskad. (Om det är svårt att hitta kodgranskare, så rekommenderar jag mutor i form av riktig öl) Ni ska dessutom granska en annan grupps kod

## Inlämning via ithsdistans
* Granskningsprotokollet ska lämnas in i som en fil eller i en zippad fil på ithsdistans
* Granskningsprotokollet ska innehålla:
 * vilken grupps laboration ni har granskat
 * URL till GitHub-repot
 * minst tre saker som ni tycker att gruppen har gjort bra
 * en eller två saker som man hade kunnat göra annorlunda eller bättre
 * andra reflektioner
* Uppgiften lämnas in på ithsdistans
* Eftersom det är en gruppuppgift så lämnar en partner in koden
* Den partner som inte lämnar in kod skall istället lämna in en kommentar. Till exempel
“Jobbar i grupp med Pontus Lindgren”

**Kriterier för godkänt**
* programmet går att köra i alla åtta testfall utan att det kraschar
* när puben stänger ska alla gäster ha gått hem och det ska vara samma antal glas och lediga stolar som när den öppnade
* en tråd per agent
* understrykna händelser ska läggas till först i rätt ListBox
* alla besökare får dricka öl till slut
* ni har granskat en annan grupps kod
* godkänd demonstration för läraren

**Kriterier för väl godkänt**
* väl godkänd demonstration för läraren
* det går att ändra simuleringens hastighet i det grafiska gränssnittet
* alla besökare betjänas i den ordning de anlände till baren
* väl strukturerad, lättläst och tydlig kod
* följer objektorienterade principer vid design av klasser
