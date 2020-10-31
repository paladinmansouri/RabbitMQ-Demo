namespace CommonCode
{
    public class CalculationRequest
    {
        public int Number1 { get; set; }
        public int Number2 { get; set; }
        public OperationType OperationType { get; set; }

        public CalculationRequest()
        {
            
        }

        public CalculationRequest(int number1,int number2,OperationType operationType)
        {
            Number1 = number1;
            Number2 = number2;
            OperationType = operationType;
        }

        public override string ToString()
        {
            return Number1 + (OperationType == OperationType.Add ? "+" : "-") + Number2;
        }
    }
}
