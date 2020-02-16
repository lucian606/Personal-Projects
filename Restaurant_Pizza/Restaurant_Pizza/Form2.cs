using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

namespace Restaurant_Pizza
{
    public partial class MenuForm : Form
    {
        AuthenticationForm parentForm;
        PizzaRestaurant domino;
        private Client client;
        private float totalPrice = 0;
        private float margheritaCost;
        private float carnivoraCost;
        private float staggioniCost;
        private float formaggiCost;
        public MenuForm(String name, float balance, AuthenticationForm parentForm)
        {
            this.parentForm = parentForm;
            client = new Client(name, balance);
            InitializeComponent();
            balanceBox.Text = balance.ToString();
            nameBox.Text = name;
            domino = PizzaRestaurant.getInstance();
            margheritaCost = domino.pizzaFactory("Margherita").getPrice();
            carnivoraCost = domino.pizzaFactory("Carnivora").getPrice();
            staggioniCost = domino.pizzaFactory("QuattroStaggioni").getPrice();
            formaggiCost = domino.pizzaFactory("QuattroFormaggi").getPrice();
            margheritaPrice.Text = margheritaCost.ToString();
            carnivoraPrice.Text = carnivoraCost.ToString();
            staggioniPrice.Text = staggioniCost.ToString();
            formaggiPrice.Text = formaggiCost.ToString();
            offerBox.Text = domino.getOffer().generateOffer().show();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        public abstract class Pizza
        {
            protected int quantity;
            protected ArrayList ingredients;
            protected float price;
            public Pizza(int quantity, ArrayList ingredients, float price)
            {
                this.quantity = quantity;
                this.ingredients = ingredients;
                this.ingredients.Add("Mozzarella");
                this.ingredients.Add("Tomato Sauce");
                if (typeof(OddDaysStrategy).IsInstanceOfType(PizzaRestaurant.getInstance().getOffer()))
                    price = price / 2;
                this.price = price;
            }
            public void showIngredients()
            {
                Console.WriteLine(ingredients);
            }
            public float getPrice()
            {
                return price;
            }

        }
        public class Carnivora : Pizza
        {
            public Carnivora() : base(450, new ArrayList(), 22.5f)
            {
                ingredients.Add("Prosciutto");
                ingredients.Add("Bacon");
                ingredients.Add("Pepperoni");
            }
        }
        public class QuattroStaggioni : Pizza
        {
            public QuattroStaggioni() : base(380, new ArrayList(), 21f)
            {
                ingredients.Add("Prosciutto");
                ingredients.Add("Olives");
                ingredients.Add("Pepper");
                ingredients.Add("Mushrooms");
            }
        }
        public class Margherita : Pizza
        {
            public Margherita() : base(350, new ArrayList(), 17.5f)
            {
            }
        }
        public class QuattroFormaggi : Pizza
        {
            public QuattroFormaggi() : base(400, new ArrayList(), 20.5f)
            {
                ingredients.Add("Gorgonzola");
                ingredients.Add("Blue Cheese");
                ingredients.Add("Parmesan");
                if (typeof(SundayStrategy).IsInstanceOfType(PizzaRestaurant.getInstance().getOffer()))
                    price = price * 0.25f;
            }
        }
        public interface Observable
        {
            void addObserver(Client client);
            void removeObserver(Client client);
            void notifyAllObservers();
        }
        public class PizzaRestaurant : Observable
        {
            private static PizzaRestaurant instance = null;
            public ArrayList clients;
            private Strategy offer;
            private String day = System.DateTime.Now.DayOfWeek.ToString();
            private PizzaRestaurant()
            {
                clients = new ArrayList();
                if (day.Equals("Monday") || day.Equals("Wednesday") || day.Equals("Friday"))
                    offer = new OddDaysStrategy();
                else if (day.Equals("Tuesday") || day.Equals("Thursday") || day.Equals("Saturday"))
                    offer = new EvenDaysStrategy();
                else
                    offer = new SundayStrategy();
            }
            public static PizzaRestaurant getInstance()
            {
                if (instance == null)
                    instance = new PizzaRestaurant();
                return instance;
            }
            public Pizza pizzaFactory(String type)
            {
                if (type.Equals("Margherita"))
                    return new Margherita();
                else if (type.Equals("QuattroFormaggi"))
                    return new QuattroFormaggi();
                else if (type.Equals("QuattroStaggioni"))
                    return new QuattroStaggioni();
                else if (type.Equals("Carnivora"))
                    return new Carnivora();
                else
                    return null;
            }
            public void addObserver(Client client)
            {
                if (clients.Contains((Object)client) == false)
                    clients.Add(client);
            }
            public void removeObserver(Client client)
            {
                if (clients.Contains(client) == true)
                    clients.Remove(client);
            }
            public void notifyAllObservers()
            {
                for (int i = 0; i < clients.Count; i++)
                {
                    Client client = (Client)clients[i];
                    Notification notification = offer.generateOffer();
                    client.update(notification);
                }
            }
            public Strategy getOffer()
            {
                return offer;
            }
        }
        public interface Observer
        {
            void update(Notification notification);
        }
        public class Client : Observer
        {
            private String name;
            private ArrayList orders;
            private float balance;
            private ArrayList notifications;
            public Client(String name, float balance)
            {
                this.name = name;
                this.balance = balance;
                orders = new ArrayList();
                notifications = new ArrayList();
            }
            public void update(Notification notification)
            {
                notifications.Add(notification);
            }
            public ArrayList getOrders()
            {
                return orders;
            }
            public float getBalance()
            {
                return balance;
            }
            public ArrayList getNotifications()
            {
                return notifications;
            }
            public void addOrder(Pizza pizza)
            {
                orders.Add(pizza);
            }
            public void printOrders()
            {
                foreach (Pizza pizza in orders)
                    Console.WriteLine(pizza.GetType().Name + " " + pizza.getPrice());
            }
            public void printNotifications()
            {
                foreach (Notification notification in notifications)
                    notification.show();
            }
            public void confirmOrder()
            {
                float total = 0;
                int margheritaCounter = 0;
                int carnivoraCounter = 0;
                int staggioniCounter = 0;
                int formaggiCounter = 0;
                int offerCheck=0;
                for (int i = 0; i < orders.Count; i++)
                {
                    Pizza pizza = (Pizza)orders[i];
                    if (typeof(OddDaysStrategy).IsInstanceOfType(PizzaRestaurant.getInstance().getOffer()))
                        total += (pizza.getPrice() / 2);
                    else if (typeof(EvenDaysStrategy).IsInstanceOfType(PizzaRestaurant.getInstance().getOffer()))
                    {
                        if (typeof(Margherita).IsInstanceOfType(pizza))
                        {
                            margheritaCounter++;
                            offerCheck = margheritaCounter;
                        }
                        else if (typeof(Carnivora).IsInstanceOfType(pizza))
                        {
                            carnivoraCounter++;
                            offerCheck = carnivoraCounter;
                        }
                        else if (typeof(QuattroFormaggi).IsInstanceOfType(pizza))
                        {
                            formaggiCounter++;
                            offerCheck = formaggiCounter;
                        }
                        else if (typeof(QuattroStaggioni).IsInstanceOfType(pizza))
                        {
                            staggioniCounter++;
                            offerCheck = staggioniCounter;
                        }
                        if (offerCheck % 3 != 0)
                            total += pizza.getPrice();
                    }
                    else
                    {
                        if (typeof(QuattroFormaggi).IsInstanceOfType(pizza))
                            total += (pizza.getPrice() * 0.25f);
                    }
                }
                if (total > balance)
                    MessageBox.Show("Insufficient funds!");
                else
                {
                    balance -= total;
                    MessageBox.Show("Thank you for your purchase!\nHere is your change" + " " + balance);
                }
            }
        }
        public interface Notification
        {
            String show();
        }
        public class SaleNotification : Notification
        {
            public String show()
            {
                return "Every Pizza is at half price today!";
            }
        }
        public class BundleNotification : Notification
        {
            public String show()
            {
                return "Buy 2 Pizzas get 1 free!";
            }
        }
        public class CheesyNotification : Notification
        {
            public String show()
            {
                return "Get a Quattro Formaggi Pizza for 25% of the price!";
            }
        }
        public interface Strategy
        {
            Notification generateOffer();
        }
        public class OddDaysStrategy : Strategy
        {
            public Notification generateOffer()
            {
                return new SaleNotification();
            }
        }
        public class EvenDaysStrategy : Strategy
        {
            public Notification generateOffer()
            {
                return new BundleNotification();
            }
        }
        public class SundayStrategy : Strategy
        {
            public Notification generateOffer()
            {
                return new CheesyNotification();
            }
        }

        private void margheritaCounter_ValueChanged(object sender, EventArgs e)
        {
            int temp=(int)margheritaCounter.Value;
            if (typeof(EvenDaysStrategy).IsInstanceOfType(PizzaRestaurant.getInstance().getOffer())) 
                {
                    if (temp % 3 == 0)
                    {
                        temp--;
                    }
                }
            totalPrice = margheritaCost * temp + staggioniCost * (int)staggioniCounter.Value +
                carnivoraCost * (int)carnivoraCounter.Value + formaggiCost * (int)formaggiCounter.Value;
            costBox.Text = totalPrice.ToString();
        }

        private void carnivoraCounter_ValueChanged(object sender, EventArgs e)
        {
            int temp = (int)carnivoraCounter.Value;
            if (typeof(EvenDaysStrategy).IsInstanceOfType(PizzaRestaurant.getInstance().getOffer()))
            {
                if (temp % 3 == 0)
                {
                    temp--;
                }
            }
            totalPrice = margheritaCost * (int)margheritaCounter.Value + staggioniCost * (int)staggioniCounter.Value +
                carnivoraCost * temp + formaggiCost * (int)formaggiCounter.Value;
            costBox.Text = totalPrice.ToString();
        }

        private void formaggiCounter_ValueChanged(object sender, EventArgs e)
        {
            int temp = (int)formaggiCounter.Value;
            if (typeof(EvenDaysStrategy).IsInstanceOfType(PizzaRestaurant.getInstance().getOffer()))
            {
                if (temp % 3 == 0)
                {
                    temp--;
                }
            }
            totalPrice = margheritaCost * (int)margheritaCounter.Value + staggioniCost * (int)staggioniCounter.Value +
                carnivoraCost * (int)carnivoraCounter.Value + formaggiCost * temp;
            costBox.Text = totalPrice.ToString();
        }

        private void staggioniCounter_ValueChanged(object sender, EventArgs e)
        {
            int temp = (int)staggioniCounter.Value;
            if (typeof(EvenDaysStrategy).IsInstanceOfType(PizzaRestaurant.getInstance().getOffer()))
            {
                if (temp % 3 == 0)
                {
                    temp--;
                }
            }
            totalPrice = margheritaCost * (int)margheritaCounter.Value + staggioniCost * temp +
                carnivoraCost * (int)carnivoraCounter.Value + formaggiCost * (int)formaggiCounter.Value;
            costBox.Text = totalPrice.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int i;
            for (i = 1; i <= margheritaCounter.Value; i++)
                client.addOrder(domino.pizzaFactory("Margherita"));
            for (i = 1; i <= carnivoraCounter.Value; i++)
                client.addOrder(domino.pizzaFactory("Carnivora"));
            for (i = 1; i <= formaggiCounter.Value; i++)
                client.addOrder(domino.pizzaFactory("QuattroFormaggi"));
            for (i = 1; i <= staggioniCounter.Value; i++)
                client.addOrder(domino.pizzaFactory("QuattroStaggioni"));
            client.confirmOrder();
            balanceBox.Text = client.getBalance().ToString();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.Close();
            parentForm.Show();
        }
    }
}
