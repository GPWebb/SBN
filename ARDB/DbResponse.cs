namespace AlpineRed.DB
{
    public class DbResponse<T>
    {
        public T Data { get; set; }

        public int ReturnCode { get; set; }
    }
}