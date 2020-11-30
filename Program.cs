using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace DatabaseLessBenchmark
{
	class Program
	{
		static void Main(string[] args)
		{

			int nRecord = 10000;
			var emptyRecord = new byte[Record.LenRecord];
			if (File.Exists("data.bin"))
				File.Delete("data.bin");
			using (var stream = File.Open("data.bin", FileMode.OpenOrCreate))
			{
				// ========= create a empty database for nRecord =========
				for (int i = 0; i < nRecord - 1; i++)
				{
					stream.Write(emptyRecord, 0, emptyRecord.Length);
				}

				Stopwatch stopWatch = new Stopwatch();

				// ========= start benchmark save record in progressive  =========
				stopWatch.Start();
				var counter = 0;
				for (int loop = 0; loop < 100; loop++)
				{
					for (int id = 0; id < nRecord; id++)
					{
						counter++;
						var record = GenerateRandomRecord(id);
						record.Save(stream);//write
					}
				}
				stopWatch.Stop();
				Console.WriteLine("progressive write = " + stopWatch.Elapsed.TotalSeconds + " sec. for " + counter + " records");

				// ========= start benchmark save 1.000.000 records in random position  =========
				stopWatch.Start();
				for (int i = 0; i < 1000000; i++)
				{
					var id = rnd.Next(0, nRecord - 1);
					var record = GenerateRandomRecord(id);
					record.Save(stream);//write
				}
				stopWatch.Stop();
				Console.WriteLine("random write = " + stopWatch.Elapsed.TotalSeconds + " sec. for 1.000.000 records");

				// ========= start benchmark load 1.000.000 records in random position  =========
				stopWatch.Reset();
				stopWatch.Start();
				for (int i = 0; i < 1000000; i++)
				{
					var id = rnd.Next(0, nRecord - 1);
					Record record = new Record(stream, id); //load a record id
				}
				stopWatch.Stop();
				Console.WriteLine("random read = " + stopWatch.Elapsed.TotalSeconds + " sec. for 1.000.000 records");

				// ========= start benchmark save e load 1.000.000 records in random position  =========
				stopWatch.Reset();
				stopWatch.Start();
				for (int i = 0; i < 1000000; i++)
				{
					var id = rnd.Next(0, nRecord - 1);
					Record record = new Record(stream, id); //load a record id
					id = rnd.Next(0, nRecord - 1);
					record.Id = id;
					record.Save(stream); //write
				}
				stopWatch.Stop();
				Console.WriteLine("random read/write = " + stopWatch.Elapsed.TotalSeconds + " sec. for 1.000.000 records (1 million + 1 million)");

				// ========= start benchmark save e single field 1.000.000 time (save date field only)  =========
				stopWatch.Reset();
				stopWatch.Start();
				var rec = GenerateRandomRecord(0);
				for (int i = 0; i < 1000000; i++)
				{
					rec.date = DateTime.Now;
					rec.Id = rnd.Next(0, nRecord - 1);
					rec.SaveFiledDate(stream);
				}
				stopWatch.Stop();
				Console.WriteLine("save date field = " + stopWatch.Elapsed.TotalSeconds + " sec. for 1.000.000 of fields saved");


				Console.WriteLine("\npress any key to exit the process...");
				Console.ReadKey();




			}



		}


		static public Record GenerateRandomRecord(int id)
		{
			return new Record()
			{
				Id = id,
				x = (ulong)rnd.Next(),
				y = (ulong)rnd.Next(),
				latitude = rnd.NextDouble(),
				longitude = rnd.NextDouble(),
				valitate = (rnd.Next() & 1) != 0,
				location = "usa",
				d1 = (uint)rnd.Next(),
				d2 = (uint)rnd.Next(),
				d3 = (uint)rnd.Next(),
				d4 = (uint)rnd.Next(),
				d5 = (uint)rnd.Next(),
				d6 = (uint)rnd.Next(),
				d7 = (uint)rnd.Next(),
				d8 = (uint)rnd.Next(),
				d9 = (uint)rnd.Next(),
				d10 = (uint)rnd.Next(),
				name = "pinco",
				surname = "pallino" + id,
				role = Record.roleType.user,
				note = "text note text note text note text note text note text note text note text note text note text note text note text note text note text note text note text note text note text note text note text note text note text note text note text note text note text note text note text note text note text note text note text note text note text note text note text note text note text note text note text note text note text note text note text note text note text note text note text note text note text note ",
				month01 = rnd.NextDouble(),
				month02 = rnd.NextDouble(),
				month03 = rnd.NextDouble(),
				month04 = rnd.NextDouble(),
				month05 = rnd.NextDouble(),
				month06 = rnd.NextDouble(),
				month07 = rnd.NextDouble(),
				month08 = rnd.NextDouble(),
				month09 = rnd.NextDouble(),
				month10 = rnd.NextDouble(),
				month11 = rnd.NextDouble(),
				month12 = rnd.NextDouble(),
				date = DateTime.Now
			};
		}

		static public Random rnd = new Random();

		public class Record
		{
			public Record()
			{
			}
			public Record(byte[] data)
			{
				input(data);
			}
			public Record(FileStream stream, int id) //load
			{
				Load(stream, id);
			}
			public void Save(FileStream stream)
			{
				var data = output();
				var offset = Id * LenRecord;
				stream.Seek(offset, SeekOrigin.Begin);
				stream.Write(data, 0, data.Length);
			}
			public void Load(FileStream stream, int id)
			{
				var offset = id * LenRecord;
				stream.Seek(offset, SeekOrigin.Begin);
				byte[] data = new byte[LenRecord];
				var result = stream.Read(data, 0, data.Length);
				input(data);
			}
			public const int LenRecord = 1482;
			private byte[] output()
			{
				byte[] result = new byte[LenRecord];
				BitConverter.GetBytes(Id).CopyTo(result, 0); //[0 - 3 bytes]
				BitConverter.GetBytes(x).CopyTo(result, 4); //[4 - 11 bytes]
				BitConverter.GetBytes(y).CopyTo(result, 12); //[12 - 19 bytes]
				BitConverter.GetBytes(latitude).CopyTo(result, 20); //[20 - 27 bytes]
				BitConverter.GetBytes(longitude).CopyTo(result, 28); //[28 - 35 bytes]
				(valitate ? new byte[] { 1 } : new byte[] { 0 }).CopyTo(result, 36); //[36 bytes]
				Encoding.ASCII.GetBytes(location).CopyTo(result, 37); //[37 - 136 bytes] - (100 chars)
				BitConverter.GetBytes(d1).CopyTo(result, 137); //[137 - 140 bytes]
				BitConverter.GetBytes(d2).CopyTo(result, 141); //[141 - 144 bytes]
				BitConverter.GetBytes(d3).CopyTo(result, 145); //[145 - 148 bytes]
				BitConverter.GetBytes(d4).CopyTo(result, 149); //[149 - 152 bytes]
				BitConverter.GetBytes(d5).CopyTo(result, 153); //[153 - 156 bytes]
				BitConverter.GetBytes(d6).CopyTo(result, 157); //[157 - 160 bytes]
				BitConverter.GetBytes(d7).CopyTo(result, 161); //[161 - 164 bytes]
				BitConverter.GetBytes(d8).CopyTo(result, 165); //[165 - 168 bytes]
				BitConverter.GetBytes(d9).CopyTo(result, 169); //[169 - 172 bytes]
				BitConverter.GetBytes(d10).CopyTo(result, 173); //[173 - 176 bytes]
				Encoding.ASCII.GetBytes(name).CopyTo(result, 177); //[177 - 276 bytes] - (100 chars)
				Encoding.ASCII.GetBytes(surname).CopyTo(result, 277); //[277 - 376 bytes] - (100 chars)
				new byte[] { (byte)role }.CopyTo(result, 377); //[377 bytes]
				Encoding.ASCII.GetBytes(note).CopyTo(result, 378); //[378 - 1377 bytes] - (1000 chars)
				BitConverter.GetBytes(month01).CopyTo(result, 1378); //[1378 - 1385 bytes]
				BitConverter.GetBytes(month02).CopyTo(result, 1386); //[1386 - 1393 bytes]
				BitConverter.GetBytes(month03).CopyTo(result, 1394); //[1394 - 1401 bytes]
				BitConverter.GetBytes(month04).CopyTo(result, 1402); //[1402 - 1409 bytes]
				BitConverter.GetBytes(month05).CopyTo(result, 1410); //[1410 - 1417 bytes]
				BitConverter.GetBytes(month06).CopyTo(result, 1418); //[1418 - 1425 bytes]
				BitConverter.GetBytes(month07).CopyTo(result, 1426); //[1426 - 1433 bytes]
				BitConverter.GetBytes(month08).CopyTo(result, 1434); //[1434 - 1441 bytes]
				BitConverter.GetBytes(month09).CopyTo(result, 1442); //[1442 - 1449 bytes]
				BitConverter.GetBytes(month10).CopyTo(result, 1450); //[1450 - 1457 bytes]
				BitConverter.GetBytes(month11).CopyTo(result, 1458); //[1458 - 1465 bytes]
				BitConverter.GetBytes(month12).CopyTo(result, 1466); //[1466 - 1473 bytes]
				BitConverter.GetBytes(date.Ticks).CopyTo(result, 1474); //[1474 - 1481 bytes]
				return result;
			}
			private void input(byte[] data)
			{
				Id = BitConverter.ToInt32(data, 0);
				x = BitConverter.ToUInt64(data, 4);
				y = BitConverter.ToUInt64(data, 12);
				latitude = BitConverter.ToDouble(data, 20);
				longitude = BitConverter.ToDouble(data, 28);
				valitate = data[36] != 0;
				location = Encoding.ASCII.GetString(data, 37, 100);
				d1 = BitConverter.ToUInt32(data, 137);
				d2 = BitConverter.ToUInt32(data, 141);
				d3 = BitConverter.ToUInt32(data, 145);
				d4 = BitConverter.ToUInt32(data, 149);
				d5 = BitConverter.ToUInt32(data, 153);
				d6 = BitConverter.ToUInt32(data, 157);
				d7 = BitConverter.ToUInt32(data, 161);
				d8 = BitConverter.ToUInt32(data, 165);
				d9 = BitConverter.ToUInt32(data, 169);
				d10 = BitConverter.ToUInt32(data, 173);
				name = Encoding.ASCII.GetString(data, 177, 100);
				surname = Encoding.ASCII.GetString(data, 277, 100);
				role = (roleType)data[377];
				note = Encoding.ASCII.GetString(data, 378, 1000);
				month01 = BitConverter.ToDouble(data, 1378);
				month02 = BitConverter.ToDouble(data, 1386);
				month03 = BitConverter.ToDouble(data, 1394);
				month04 = BitConverter.ToDouble(data, 1402);
				month05 = BitConverter.ToDouble(data, 1410);
				month06 = BitConverter.ToDouble(data, 1418);
				month07 = BitConverter.ToDouble(data, 1426);
				month08 = BitConverter.ToDouble(data, 1434);
				month09 = BitConverter.ToDouble(data, 1442);
				month10 = BitConverter.ToDouble(data, 1450);
				month11 = BitConverter.ToDouble(data, 1458);
				month12 = BitConverter.ToDouble(data, 1466);
				date = new DateTime(BitConverter.ToInt64(data, 1474));
			}

			public int Id;
			public ulong x;
			public ulong y;
			public double latitude;
			public double longitude;
			public bool valitate;
			public string location;
			public uint d1;
			public uint d2;
			public uint d3;
			public uint d4;
			public uint d5;
			public uint d6;
			public uint d7;
			public uint d8;
			public uint d9;
			public uint d10;
			public string name;
			public string surname;
			public roleType role;
			public enum roleType : byte
			{
				user,
				manager,
				admin,
				boss
			}
			public string note;
			public double month01;
			public double month02;
			public double month03;
			public double month04;
			public double month05;
			public double month06;
			public double month07;
			public double month08;
			public double month09;
			public double month10;
			public double month11;
			public double month12;
			public DateTime date;
			public void SaveFiledDate(FileStream stream)
			{
				var data = BitConverter.GetBytes(date.Ticks);
				var offset = Id * LenRecord + 1474; //[1474 - 1481 bytes]
				stream.Seek(offset, SeekOrigin.Begin);
				stream.Write(data, 0, data.Length);
			}
		}


	}
}
