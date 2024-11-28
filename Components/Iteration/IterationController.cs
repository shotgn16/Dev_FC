namespace ForestChurches.Components.Iteration
{
    public class IterationController
    {
        private static int Incrementor;
        public static int Value;

        public static Task IterateThrough()
        {
            if (Incrementor == 0)
            {
                Incrementor++;
                Value = 0;
            }

            else if (Incrementor > 0)
            {
                Incrementor++;
                Value++;
            }

            return Task.CompletedTask;
        }

        public void Dispose() => GC.Collect();
    }
}
