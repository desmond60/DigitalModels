namespace DigitalModels.other;

public struct Receiver      /// Структура приемника
{
    public double x { get; set; }  /// Координата X 
    public double y { get; set; }  /// Координата Y
    public double z { get; set; }  /// Координата Z

    public Receiver(double _x, double _y, double _z) {
        this.x = _x; this.y = _y; this.z = _z;
    }

    public Receiver(double[] point) {
        this.x = point[0]; this.y = point[1]; this.z = point[2];
    }

    public void Deconstruct(out double x, 
                            out double y,
                            out double z) 
    {
        x = this.x;
        y = this.y;
        z = this.z;
    }

    public override string ToString() => $"{x,20} {y,24} {z,26}";
}

public struct Source      /// Структура источника
{
    public double x { get; set; }  /// Координата X 
    public double y { get; set; }  /// Координата Y
    public double z { get; set; }  /// Координата Z

    public Source(double _x, double _y, double _z) {
        this.x = _x; this.y = _y; this.z = _z;
    }

    public Source(double[] point) {
        this.x = point[0]; this.y = point[1]; this.z = point[2];
    }

    public void Deconstruct(out double x, 
                            out double y,
                            out double z) 
    {
        x = this.x;
        y = this.y;
        z = this.z;
    }

    public override string ToString() => $"{x,20} {y,24} {z,26}";
}

public static class Helper
{
    //* Вычисление нормы вектора
    public static double Norma(Vector<double> vec) {
        double res = 0;
        for (int i = 0; i < vec.Length; i++)
            res += vec[i]*vec[i];
        return Sqrt(res);
    }

    //* Расстояние от точки измерения до электрода
    public static double interval(Receiver R, Source S) {
        return Sqrt(Pow(S.x - R.x, 2) + Pow(S.y - R.y, 2) + Pow(S.z - R.z, 2));
    }

    //* Расчет потенциала
    public static double potential(Source A, Source B, Receiver M, Receiver N, double I) {
        double diff = (1 / interval(M, B) - 1 / interval(M, A)) - (1 / interval(N, B) - 1 / interval(N, A));
        return I / (2.0 * PI * sigma) * diff;
    }

    //* Расчет diff потенциала
    public static double diff_potential(Source A, Source B, Receiver M, Receiver N, double I) {
        double diff = (1 / interval(M, B) - 1 / interval(M, A)) - (1 / interval(N, B) - 1 / interval(N, A));
        return -I / (2.0 * PI * sigma * sigma) * diff;
    }

    //* Расчет суммарного потенциала (1 приемник от 3 источников)
    public static double summPotential(Source[] sources, Receiver receiver, Receiver next_receiver, Vector<double> I) {
        double v1 = potential(sources[0], sources[1], receiver, next_receiver, I[0]);
        double v2 = potential(sources[2], sources[3], receiver, next_receiver, I[1]);
        double v3 = potential(sources[4], sources[5], receiver, next_receiver, I[2]);
        return v1 + v2 + v3;
    }

    //* Окно помощи при запуске (если нет аргументов или по команде)
    public static void ShowHelp() {
        WriteLine("----Команды----                        \n" + 
        "-help             - показать справку             \n" + 
        "-i                - входной файл                 \n");
    }
}