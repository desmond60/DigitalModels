namespace DigitalModels;
public static class Function
{
    public static uint           numberFunc;     /// Номер задачи
    public static double         sigma;          /// Значение Sigma
    public static Vector<double> Absolut_I;      /// Истинное решение 
    public static Vector<double> omega;          /// Значние omega = 1 / V (потенциал)
    public static Vector<double> I_init;         /// Начальное приближение

    //* Инициализация переменных
    public static void Init(uint numF) {
        numberFunc = numF;

        switch(numberFunc) {
            case 1:
                sigma     = 0.1;          
                Absolut_I = new Vector<double>(new []{1.0, 2.0, 3.0});
                omega     = new Vector<double>(3);
                I_init    = new Vector<double>(new []{0.1, 0.1, 0.1});
            break;
        }
    }
}