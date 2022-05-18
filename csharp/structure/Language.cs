namespace csharp_ej1
{
    class Language
    {
        private string name;
        private long repoAmmount;
        private double rating;

        public Language(string name, long repoAmmount, double rating)
        {
            this.name = name;
            this.repoAmmount = repoAmmount;
            this.rating = rating;
        }

        public string getName()
        {
            return this.name;
        }

        public long getRepoAmmount()
        {
            return this.repoAmmount;
        } 

        public double getRating()
        {
            return this.rating;
        }
    }
}