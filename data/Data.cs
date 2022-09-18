namespace DigitalModels;
public class Data
{
    //* Данные для задачи
    public double[][] Receivers   { get; set; }    /// Положение приемников
    public double[][] Sources     { get; set; }    /// Положение источников  

    public uint N { get; set; }  /// Номер решаемой задачи

    //* Деконструктор (для источников и приемников)
    public void Deconstruct(out Receiver[] receivers, 
                            out Source[]   sources) {
        receivers = new Receiver[Receivers.Length];
        sources   = new Source[Sources.Length];

        for (int i = 0; i < Receivers.Length; i++)
            receivers[i] = new Receiver(Receivers[i]);

        for (int i = 0; i < sources.Length; i++)
            sources[i] = new Source(Sources[i]);
    }
}