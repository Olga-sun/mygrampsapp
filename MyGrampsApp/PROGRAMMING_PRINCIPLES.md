1. DRY (Don't Repeat Yourself)
Опис: Винесення логіки роботи з базою даних в окремий сервіс дозволяє не дублювати код підключення у кожному вікні.

Доказ: Усі методи доступу до даних та SQL-запити реалізовані в єдиному файлі DatabaseService.cs. [DatabaseService.cs](https://github.com/Olga-sun/mygrampsapp/blob/master/MyGrampsApp/Services/DatabaseService.cs)

2. Single Responsibility Principle (SRP)
Опис: Кожен клас відповідає за одну конкретну задачу. Наприклад, класи в папці Models описують лише структуру даних, а ViewModels — логіку взаємодії з інтерфейсом.

Доказ: Клас Person.cs описує лише сутність користувача. https://github.com/Olga-sun/mygrampsapp/blob/61b5ce209e370fe5b11e6deb1788207c3bca5937/MyGrampsApp/Models/Person.cs#L7

3. Encapsulation (Інкапсуляція)
Опис: Використання приватних полів та публічних властивостей (properties) для контролю доступу до даних.

Доказ 1 (Приватні поля): У класі DatabaseService рядок підключення до бази даних захищений модифікатором private readonly, що унеможливлює його зміну ззовні.

Посилання: https://github.com/Olga-sun/mygrampsapp/blob/61b5ce209e370fe5b11e6deb1788207c3bca5937/MyGrampsApp/Services/DatabaseService.cs#L11

Доказ 2 (Публічні властивості): У моделі Person дані доступні через публічні властивості, а логіка формування повного імені прихована всередині обчислювальної властивості FullName.

Посилання: https://github.com/Olga-sun/mygrampsapp/blob/61b5ce209e370fe5b11e6deb1788207c3bca5937/MyGrampsApp/Models/Person.cs#L9-L21

Доказ 3 (Методи обробки): У MainViewModel методи для видалення або завантаження даних мають модифікатор private або protected, що обмежує доступ до внутрішньої логіки роботи вікна.

Посилання: https://github.com/Olga-sun/mygrampsapp/blob/61b5ce209e370fe5b11e6deb1788207c3bca5937/MyGrampsApp/ViewModels/MainViewModel.cs#L80

4. KISS (Keep It Simple, Stupid)

Опис: Замість складних методів конкатенації або створення окремих класів для форматування, використовується проста інтерполяція рядків прямо у властивості моделі.

Доказ: Лаконічна реалізація властивості FullName у класі Person.

Посилання: https://github.com/Olga-sun/mygrampsapp/blob/61b5ce209e370fe5b11e6deb1788207c3bca5937/MyGrampsApp/Models/Person.cs#L21

5. Composition over Inheritance (Композиція замість успадкування)
Замість того, щоб успадковувати MainViewModel від якогось «базового класу бази даних», ми включаємо сервіс як поле.

Опис: Клас не успадковує функціонал роботи з БД, а використовує об'єкт-сервіс через приватне поле. Це робить код гнучким.

Доказ: Використання приватного поля _dbService типу DatabaseService всередині MainViewModel.

Посилання: https://github.com/Olga-sun/mygrampsapp/blob/61b5ce209e370fe5b11e6deb1788207c3bca5937/MyGrampsApp/ViewModels/MainViewModel.cs#L21

6. Separation of Concerns (Розділення відповідальності)
Це глобальний принцип, на якому базується твій патерн MVVM.

Опис: Проект чітко розділений на логічні рівні: дані (Models), логіка (ViewModels), інтерфейс (Views) та інфраструктура (Services). Це дозволяє змінювати дизайн вікна, не чіпаючи код бази даних.

Доказ: Структура папок у рішенні проекту.

Посилання: [Структура папок MyGrampsApp](https://github.com/Olga-sun/mygrampsapp/tree/61b5ce209e370fe5b11e6deb1788207c3bca5937/MyGrampsApp)

