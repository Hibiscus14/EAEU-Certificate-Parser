# EAEU Certificate Parser

Простой консольный парсер сертификатов соответствия с сайта ЕАЭС.

---

## О проекте

Этот проект скачивает и парсит данные сертификатов с [tech.eaeunion.org](https://tech.eaeunion.org/tech/registers/35-1/ru/registryList/conformityDocs)  
Поддерживаются сертификаты из России, Казахстана, Кыргызстана, Армении и Белоруссии.  
Данные сохраняются в локальную базу SQLite или PostgreSQL (конфиг в `appsettings.json`).

---

## Особенности

- Поддержка двух СУБД: SQLite и PostgreSQL
- Настраиваемое соединение через `appsettings.json`
- Сохранение данных с проверкой уникальности (`doc_id`)

---

## Быстрый старт

1. Склонируйте репозиторий  
   ```bash
   git clone https://github.com/yourusername/eaeu-certificate-parser.git
   cd eaeu-certificate-parser 
   ```

2. Отредактируйте appsettings.json для настройки подключения к базе:

```bash
{
  "Database": {
    "Provider": "SQLite",
    "ConnectionString": "Data Source=certificates.db"
  }
}
```
или для PostgreSQL:

```bash
{
  "Database": {
    "Provider": "PostgreSQL",
    "ConnectionString": "Host=localhost;Username=postgres;Password=yourpassword;Database=certificatesdb"
  }
}
```

3. Постройте и запустите проект:

```bash
dotnet build
dotnet run
```
## Как использовать
- Парсер автоматически загружает все страницы сертификатов с сайта
- Новые данные добавляются в базу, дубликаты игнорируются
- Для изменения частоты запуска можно добавить планировщик задач (например, Windows Task Scheduler)

## Важно
- SQLite не требует установки, PostgreSQL — да
- Для просмотра SQLite базы можно использовать DB Browser for SQLite
- Для PostgreSQL нужен локальный сервер

## Контакты
Если есть вопросы, пишите в Issues или создавайте Pull Request.
