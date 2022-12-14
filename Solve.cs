namespace DigitalModels;
public class Solve {

    public Receiver[] Receivers   { get; set; }    /// Положение приемников
    public Source[]   Sources     { get; set; }    /// Положение источников  
    Gauss gauss;                                   /// Метод решения СЛАУ (ГАУСС)

    // ****** Конструктор ***** //
    public Solve(Data data) {
        (Receivers, Sources) = data;
        gauss                = new Gauss(3, 1e-15); 
    }

    //* Основной метод решения
    public void solve() {

        // Считаем сумму потенциалов и находим omega
        Vector<double> V_fact = new Vector<double>(3);
        for (int i = 0; i < V_fact.Length; i++)
            V_fact[i] = summPotential(Sources, Receivers[2*i], Receivers[2*i + 1], Absolut_I);

        Function.omega = new Vector<double>(new[]{1 / V_fact[0], 1 / V_fact[1], 1 / V_fact[2]});

        // Построение матрицы
        Matrix matrix = CreateMatrix();

        // Построение вектора
        Vector<double> vector = CreateVector(V_fact);

        // Решение СЛАУ
        Vector<double> result = new Vector<double>(vector.Length);
        if (!gauss.solve(matrix, vector, result)) 
            regularization(matrix, vector, result);
        
        //WriteLine(result + I_init);
        Vector<double> V_res = new Vector<double>(result.Length);
        for (int i = 0; i < result.Length; i++)
            V_res[i] = summPotential(Sources, Receivers[2*i], Receivers[2*i + 1], (result + I_init));

        WriteLine((V_res - V_fact).ToString());
    }

    //* Регуляризация
    private void regularization(Matrix mat, Vector<double> vec, Vector<double> res) {
        double a_pred = 0, a_next = 1e-15;

        // Находим значение (a) при котором СЛАУ решится
        do {
            for (int i = 0; i < vec.Length; i++) 
                mat[i,i] = mat[i,i] - a_pred + a_next;
            a_pred = a_next;
            a_next *= 1.4;
        } while (!gauss.solve(mat, vec, res));
        
        // Находим наилучшее значение (a)
        double CURRENT_DISCREPANCY = Discrepancy(mat, vec, a_pred);
        a_next = a_pred * 1.4;

        do {
            for (int i = 0; i < vec.Length; i++) 
                mat[i,i] = mat[i,i] - a_pred + a_next;
            CURRENT_DISCREPANCY = Discrepancy(mat, vec, a_next);
            a_pred = a_next;
            a_next *= 1.4;
        } while (CURRENT_DISCREPANCY < 1e-15);
    }

    //* Вычисление невязки
    public double Discrepancy(Matrix mat, Vector<double> vec, double a) {
        // Копируем правую часть и матрицу
        Matrix copyMat = new Matrix(vec.Length);
        Vector<double> copyVec = new Vector<double>(vec.Length);
        Matrix.Copy(mat, copyMat);
        Vector<double>.Copy(vec, copyVec);

        // Прибавить (a)
        for (int i = 0; i < vec.Length; i++)
            copyMat[i,i] += a;

        // Решаем СЛАУ (Вызываем прям из класса с гарантией решаемости, т.к. СЛАУ исправленна)
        Vector<double> res = new Vector<double>(vec.Length);
        gauss.solve(copyMat, copyVec, res);
        
        return Norma(copyMat * res - copyVec);
    }

    //* Построение матрицы
    public Matrix CreateMatrix() {
        Matrix mat = new Matrix(3);
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                for (int k = 0; k < 3; k++) {
                    // Вычислим аналитические производные
                    // Положим I_init = 1
                    double diff_i = diff_potential(Sources[2*i], Sources[2*i + 1], Receivers[2*k], Receivers[2*k + 1], sigma);
                    double diff_j = diff_potential(Sources[2*j], Sources[2*j + 1], Receivers[2*k], Receivers[2*k + 1], sigma);
                    mat[i,j] += Pow(omega[k], 2) * diff_i * diff_j;
                }
        return mat;
    }

    //* Построение вектора
    public Vector<double> CreateVector(Vector<double> V_fact) {
        Vector<double> vec = new Vector<double>(3);

        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++) {
                double diff = diff_potential(Sources[2*i], Sources[2*i + 1], Receivers[2*j], Receivers[2*j + 1], sigma);
                double V_current = summPotential(Sources, Receivers[2*j], Receivers[2*j + 1], I_init);
                vec[i] += Pow(omega[j], 2) * diff * (V_current - V_fact[j]);
            }
        return vec;
    }
}