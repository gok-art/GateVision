public class WhiteListService
{
    private SQLiteDb _db;

    public WhiteListService(SQLiteDb db)
    {
        _db = db;
    }

    public bool IsWhiteListed(string plate)
    {
        return _db.IsPlateInWhiteList(plate);
    }
}
