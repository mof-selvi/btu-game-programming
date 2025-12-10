# Q-Learning Denemesi

- Eğitmek istediğiniz karaktere (Agent) [QLearningBrain.cs](QLearningBrain.cs) komponentini yerleştirin.
- Agent nesneniz için bir de kontrol komponenti (C#) oluşturun.
- Agent nesneniz için aksiyon metotlarınızı (hareket, saldırı, savunma, atlama vs.) belirleyin.
- Aksiyonlarınızı `QLearningBrain.RegisterAction()` ile yapay zeka sistemine tanımlayın.
- Agent nesneniz için sensörleri (oyuncuya uzaklık, mermi sayısı, yükseklik vs.) belirleyin.
- Sensörlerinizi `QLearningBrain.SetInputs()` fonksiyonuna `List` türünde gönderin.
- `QLearningBrain.DecideAction()` metodundan gelen karara göre ilgili aksiyonunuzu çağırın.
- Agent nesnenizin bulunduğu duruma göre `QLearningBrain.Reward()` (ödül) veya `QLearningBrain.Punish()` (ceza) fonksiyonlarını çağırın.
- Oyununuz çalışır durumda oldukça agent nesneniz uygun aksiyonları lehine kullanmayı öğrenecektir. Fakat bu noktaya gelmeniz fazla uzun sürebilir. Şu anki aşamada bu sistemle agent nesnenize rastgele de olsa kararlar aldırabilmeniz yeterlidir.
- Örnek kullanım için [EnemyAI.cs](EnemyAI.cs) scriptini inceleyin.

