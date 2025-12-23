using System;
using System.Collections.Generic;

namespace RogueLikeGame
{
    public class Weapon
    {
        public string Name { get; private set; }
        public int Damage { get; private set; }
        public int Durability { get; set; }
        public int CriticalChance { get; private set; }

        public Weapon(string name, int damage, int durability, int criticalChance = 10)
        {
            Name = name;
            Damage = damage;
            Durability = durability;
            CriticalChance = criticalChance;
        }

        public int Use()
        {
            if (Durability > 0)
            {
                Durability--;

                Random rnd = new Random();
                bool isCritical = rnd.Next(100) < CriticalChance;

                int finalDamage = isCritical ? Damage * 2 : Damage;

                if (isCritical)
                {
                    Console.WriteLine($"Критический удар! {Name}");
                }

                return finalDamage;
            }
            else
            {
                Console.WriteLine($"{Name} сломалось и не может быть использовано.");
                return 0;
            }
        }
    }

    public class Aid
    {
        public string Name { get; private set; }
        public int HealAmount { get; private set; }

        public Aid(string name, int healAmount)
        {
            Name = name;
            HealAmount = healAmount;
        }
    }

    public class Enemy
    {
        public string Name { get; private set; }
        public int CurrentHealth { get; private set; }
        public int MaxHealth { get; private set; }
        public Weapon Weapon { get; private set; }
        public int GoldReward { get; private set; }

        public Enemy(string name, int maxHealth, Weapon weapon, int goldReward = 0)
        {
            Name = name;
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
            Weapon = weapon;
            GoldReward = goldReward;
        }

        public int Attack()
        {
            if (Weapon != null)
            {
                return Weapon.Use();
            }
            else
            {
                return 5;
            }
        }

        public void TakeDamage(int damage)
        {
            CurrentHealth -= damage;
            if (CurrentHealth < 0) CurrentHealth = 0;
        }

        public bool IsAlive()
        {
            return CurrentHealth > 0;
        }
    }

    public class Player
    {
        public string Name { get; private set; }
        public int CurrentHealth { get; private set; }
        public int MaxHealth { get; private set; }
        public int Score { get; set; }
        public int Gold { get; set; }
        public Aid Medkit { get; set; }
        public List<Weapon> Inventory { get; private set; } = new List<Weapon>();
        public Weapon EquippedWeapon { get; private set; }

        public Player(string name, int maxHealth)
        {
            Name = name;
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
            Score = 0;
            Gold = 100;
        }

        public void Heal()
        {
            if (Medkit != null)
            {
                CurrentHealth += Medkit.HealAmount;
                if (CurrentHealth > MaxHealth)
                    CurrentHealth = MaxHealth;
                Console.WriteLine($"{Name} использовал {Medkit.Name} и восстановил здоровье до {CurrentHealth}");
            }
        }

        public int Attack()
        {
            if (EquippedWeapon != null)
            {
                int damage = EquippedWeapon.Use();
                Console.WriteLine($"{Name} атакует оружием {EquippedWeapon.Name} и наносит {damage} урона.");

                if (EquippedWeapon.Durability == 0)
                {
                    Console.WriteLine($"{EquippedWeapon.Name} сломалось!");
                    Inventory.Remove(EquippedWeapon);
                    EquippedWeapon = null;
                }
                return damage;
            }
            else
            {
                int baseDamage = 5;
                Console.WriteLine($"{Name} атакует без оружия и наносит {baseDamage} урона.");
                return baseDamage;
            }
        }

        public void EquipWeapon(int index)
        {
            if (index >= 0 && index < Inventory.Count)
            {
                EquippedWeapon = Inventory[index];
                Console.WriteLine($"{EquippedWeapon.Name} экипировано!");
            }
        }

        public void TakeDamage(int damage)
        {
            CurrentHealth -= damage;
            if (CurrentHealth < 0) CurrentHealth = 0;
            Console.WriteLine($"{Name} получил {damage} урона, здоровье: {CurrentHealth}");
        }

        public bool IsAlive()
        {
            return CurrentHealth > 0;
        }

        public void AddWeaponToInventory(Weapon weapon)
        {
            if (Inventory.Count < 5)
            {
                Inventory.Add(weapon);
                Console.WriteLine($"В инвентарь добавлено оружие: {weapon.Name} (Урон: {weapon.Damage}, Прочность: {weapon.Durability})");
            }
            else
            {
                Console.WriteLine("Инвентарь полон, оружие не добавлено.");
            }
        }

        public bool BuyItem(int cost)
        {
            if (Gold >= cost)
            {
                Gold -= cost;
                return true;
            }
            return false;
        }
    }

    public class Shop
    {
        public List<Weapon> WeaponsForSale { get; private set; } = new List<Weapon>();
        public List<Aid> AidsForSale { get; private set; } = new List<Aid>();

        public Shop()
        {
            InitializeShop();
        }

        private void InitializeShop()
        {
            WeaponsForSale.Add(new Weapon("Плазменный меч", 25, 15, 20));
            WeaponsForSale.Add(new Weapon("Лазерная винтовка", 30, 12, 15));

            AidsForSale.Add(new Aid("Большая аптечка", 50));
            AidsForSale.Add(new Aid("Энергетический напиток", 30));
        }

        public void ShowShop(Player player)
        {
            Console.WriteLine("\n=== МАГАЗИН ===");
            Console.WriteLine($"Ваше золото: {player.Gold}");
            Console.WriteLine("\nОружие:");

            for (int i = 0; i < WeaponsForSale.Count; i++)
            {
                var weapon = WeaponsForSale[i];
                int cost = weapon.Damage * 5 + weapon.Durability * 2;
                Console.WriteLine($"{i + 1}. {weapon.Name} (Урон: {weapon.Damage}, Прочность: {weapon.Durability}, Шанс крита: {weapon.CriticalChance}%) - {cost} золота");
            }

            Console.WriteLine("\nАптечки:");
            for (int i = 0; i < AidsForSale.Count; i++)
            {
                var aid = AidsForSale[i];
                int cost = aid.HealAmount * 2;
                Console.WriteLine($"{i + 1 + WeaponsForSale.Count}. {aid.Name} (+{aid.HealAmount} HP) - {cost} золота");
            }

            Console.WriteLine($"{WeaponsForSale.Count + AidsForSale.Count + 1}. Выйти из магазина");
        }
    }

    class Program
    {
        static readonly Random rnd = new Random();

        static List<string> enemyNames = new List<string> {
            "Гоблин",
            "Караванщик",
            "Наемник",
            "Жук-танк",
            "Носорог"
        };

        static List<Weapon> initialWeapons = new List<Weapon>
        {
            new Weapon("Пистолет", 15, 10),
            new Weapon("Дробовик", 20, 8),
            new Weapon("Меч", 10, 15),
            new Weapon("Лук", 12, 12),
            new Weapon("Коса", 18, 10)
        };

        static void Main(string[] args)
        {
            Console.WriteLine("Введите имя игрока:");
            string playerName = Console.ReadLine();

            Player player = new Player(playerName, 100);
            player.Medkit = new Aid("Аптечка", 20);

            Shop shop = new Shop();

            AddRandomWeaponsToInventory(player, 3);

            Console.WriteLine($"Добро пожаловать, {player.Name}! Начинаем игру...");

            bool continueGame = true;
            int roomCount = 0;

            while (continueGame && player.IsAlive())
            {
                roomCount++;
                Console.WriteLine($"\n=== КОМНАТА {roomCount} ===");

                Console.WriteLine("\nВыберите действие:\n1. Идти дальше\n2. Посетить магазин\n3. Просмотреть инвентарь\n4. Экипировать оружие\n5. Выход");
                string action = Console.ReadLine();

                switch (action)
                {
                    case "1":
                        Enemy enemy = GenerateRandomEnemy(roomCount);
                        Console.WriteLine($"\nВы встретили врага: {enemy.Name} (Здоровье: {enemy.CurrentHealth})");

                        bool enemyDefeated = Battle(player, enemy);

                        if (enemyDefeated)
                        {
                            player.Gold += enemy.GoldReward;
                            Console.WriteLine($"Получено золота: {enemy.GoldReward}");

                            if (rnd.NextDouble() < 0.3)
                            {
                                Weapon droppedWeapon = GetRandomWeaponCopy();
                                Console.WriteLine($"Враг уронил оружие: {droppedWeapon.Name}");
                                player.AddWeaponToInventory(droppedWeapon);
                            }
                        }
                        break;

                    case "2":
                        VisitShop(player, shop);
                        break;

                    case "3":
                        ShowInventory(player);
                        break;

                    case "4":
                        EquipWeaponMenu(player);
                        break;

                    case "5":
                        continueGame = false;
                        break;

                    default:
                        Console.WriteLine("Некорректный выбор!");
                        break;
                }

                if (!player.IsAlive())
                {
                    Console.WriteLine("Вы погибли. Игра окончена.");
                    continueGame = false;
                }
            }

            Console.WriteLine($"\n=== ИГРА ОКОНЧЕНА ===");
            Console.WriteLine($"Ваш счет: {player.Score}");
            Console.WriteLine($"Собранное золото: {player.Gold}");
            Console.WriteLine($"Пройдено комнат: {roomCount}");
        }

        static bool Battle(Player player, Enemy enemy)
        {
            while (player.IsAlive() && enemy.IsAlive())
            {
                Console.WriteLine($"\nВаше здоровье: {player.CurrentHealth}");
                Console.WriteLine($"{enemy.Name} здоровье: {enemy.CurrentHealth}");
                Console.WriteLine("Выберите действие:\n1. Атаковать\n2. Использовать аптечку\n3. Убежать");
                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    int damage = player.Attack();
                    enemy.TakeDamage(damage);

                    if (!enemy.IsAlive())
                    {
                        Console.WriteLine($"Вы победили {enemy.Name}!");
                        player.Score += 10;
                        return true;
                    }

                    int damageToPlayer = enemy.Attack();
                    player.TakeDamage(damageToPlayer);
                }
                else if (choice == "2")
                {
                    player.Heal();
                    int damageToPlayer = enemy.Attack();
                    player.TakeDamage(damageToPlayer);
                }
                else if (choice == "3")
                {
                    if (rnd.NextDouble() < 0.5)
                    {
                        Console.WriteLine("Вы успешно убежали от врага.");
                        return false;
                    }
                    else
                    {
                        Console.WriteLine("Враг перехватил вас!");
                        int damageToPlayer = enemy.Attack();
                        player.TakeDamage(damageToPlayer);
                    }
                }
                else
                {
                    Console.WriteLine("Некорректный выбор, пропускаем ход.");
                }
            }

            return !enemy.IsAlive();
        }

        static void VisitShop(Player player, Shop shop)
        {
            while (true)
            {
                shop.ShowShop(player);
                Console.Write("\nВыберите товар или введите номер для выхода: ");

                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    if (choice == shop.WeaponsForSale.Count + shop.AidsForSale.Count + 1)
                    {
                        Console.WriteLine("Выход из магазина.");
                        break;
                    }

                    if (choice >= 1 && choice <= shop.WeaponsForSale.Count)
                    {
                        var weapon = shop.WeaponsForSale[choice - 1];
                        int cost = weapon.Damage * 5 + weapon.Durability * 2;

                        if (player.BuyItem(cost))
                        {
                            player.AddWeaponToInventory(new Weapon(weapon.Name, weapon.Damage, weapon.Durability, weapon.CriticalChance));
                            Console.WriteLine($"Куплено: {weapon.Name}");
                        }
                        else
                        {
                            Console.WriteLine("Недостаточно золота!");
                        }
                    }
                    else if (choice > shop.WeaponsForSale.Count && choice <= shop.WeaponsForSale.Count + shop.AidsForSale.Count)
                    {
                        var aid = shop.AidsForSale[choice - shop.WeaponsForSale.Count - 1];
                        int cost = aid.HealAmount * 2;

                        if (player.BuyItem(cost))
                        {
                            player.Medkit = aid;
                            Console.WriteLine($"Куплено: {aid.Name}");
                        }
                        else
                        {
                            Console.WriteLine("Недостаточно золота!");
                        }
                    }
                }

                Console.WriteLine("\nНажмите Enter, чтобы продолжить...");
                Console.ReadLine();
            }
        }

        static void ShowInventory(Player player)
        {
            Console.WriteLine("\n=== ИНВЕНТАРЬ ===");
            Console.WriteLine($"Здоровье: {player.CurrentHealth}/{player.MaxHealth}");
            Console.WriteLine($"Золото: {player.Gold}");
            Console.WriteLine($"Счет: {player.Score}");
            Console.WriteLine($"Аптечка: {player.Medkit.Name} (+{player.Medkit.HealAmount} HP)");

            Console.WriteLine("\nОружие:");
            if (player.Inventory.Count == 0)
            {
                Console.WriteLine("Инвентарь пуст");
            }
            else
            {
                for (int i = 0; i < player.Inventory.Count; i++)
                {
                    var weapon = player.Inventory[i];
                    string equipped = (player.EquippedWeapon == weapon) ? "[Экипировано]" : "";
                    Console.WriteLine($"{i + 1}. {weapon.Name} (Урон: {weapon.Damage}, Прочность: {weapon.Durability}, Крит: {weapon.CriticalChance}%) {equipped}");
                }
            }

            Console.WriteLine("\nНажмите Enter, чтобы продолжить...");
            Console.ReadLine();
        }

        static void EquipWeaponMenu(Player player)
        {
            if (player.Inventory.Count == 0)
            {
                Console.WriteLine("В инвентаре нет оружия!");
                return;
            }

            Console.WriteLine("\nВыберите оружие для экипировки:");
            for (int i = 0; i < player.Inventory.Count; i++)
            {
                var weapon = player.Inventory[i];
                Console.WriteLine($"{i + 1}. {weapon.Name} (Урон: {weapon.Damage}, Прочность: {weapon.Durability})");
            }

            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                if (choice >= 1 && choice <= player.Inventory.Count)
                {
                    player.EquipWeapon(choice - 1);
                }
            }
        }

        static void AddRandomWeaponsToInventory(Player player, int count)
        {
            List<Weapon> copyWeapons = new List<Weapon>(initialWeapons);
            for (int i = 0; i < count; i++)
            {
                if (copyWeapons.Count == 0)
                    break;
                int index = rnd.Next(copyWeapons.Count);
                Weapon w = copyWeapons[index];
                copyWeapons.RemoveAt(index);
                player.AddWeaponToInventory(new Weapon(w.Name, w.Damage, w.Durability, w.CriticalChance));
            }

            if (player.Inventory.Count > 0)
            {
                player.EquipWeapon(0);
            }
        }

        static Weapon GetRandomWeaponCopy()
        {
            Weapon w = initialWeapons[rnd.Next(initialWeapons.Count)];
            return new Weapon(w.Name, w.Damage, w.Durability, w.CriticalChance);
        }

        static Enemy GenerateRandomEnemy(int roomNumber)
        {
            string name = enemyNames[rnd.Next(enemyNames.Count)];
            int baseHealth = 50 + (roomNumber * 3);
            int health = rnd.Next(baseHealth - 10, baseHealth + 30);
            int goldReward = 10 + (roomNumber * 2);

            Weapon enemyWeapon = null;
            if (rnd.NextDouble() < 0.8)
            {
                Weapon w = initialWeapons[rnd.Next(Math.Min(roomNumber / 2, initialWeapons.Count))];
                enemyWeapon = new Weapon(w.Name, w.Damage, w.Durability, w.CriticalChance);
            }

            return new Enemy(name, health, enemyWeapon, goldReward);
        }
    }
}