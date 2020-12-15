Program.cs, Service1.Designer.cs, Service1.cs - основполагающие файлы БД.
DataManagerConfig.cs, Errors.cs, Order.cs - в этих файлах находятся модели данных, которые нужны для работы БД, такие как пути к папкам и др.
Errors.cs определяет текст и время возникшей ошибки.
ParceToJson.cs, ParceToXMl.cs - классы, в которых нахоятся методы для создания Json и XML файлов.
Также существует класс в Parcer.cs, который в свою очередь использует интерефейс из IParce.cs, вызывает классы-парсеры JsonParser.cs и XMLParser.cs
config.json и config.xml собственно файлы конфигов Json и XML.
ErrorRepository.cs и OrderRepository.cs предоставляют репозитории с помощью которых можно взаимодествовать с БД, последний в свою очередь использует интерфейс из IOrdersRepository.cs, использующий IEnumerable<T>.
При создании БД спользовался паттерн "репозиторий". 
