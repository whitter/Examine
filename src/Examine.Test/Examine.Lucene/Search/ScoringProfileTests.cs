using System;
using System.Collections.Generic;
using System.Linq;
using Examine.Lucene.Scoring;
using Examine.Lucene.Search;
using Examine.Test.Examine.Lucene.Search.Scoring;
using Lucene.Net.Analysis.Standard;
using NUnit.Framework;

namespace Examine.Test.Examine.Lucene.Search
{
    [TestFixture]
    public class ScoringProfileTests : ExamineBaseTest
    {
        [Test]
        public void Score_No_Profiles()
        {
            var analyzer = new StandardAnalyzer(LuceneInfo.CurrentVersion);
            using (var luceneDir = new RandomIdRAMDirectory())
            using (var indexer = GetTestIndex(
                luceneDir,
                analyzer,
                new FieldDefinitionCollection(new FieldDefinition("created", "datetime"))))
            {
                indexer.IndexItems(new[]
                {
                    ValueSet.FromObject(123.ToString(), "content",
                        new
                        {
                            created = new DateTime(2000, 01, 02),
                            bodyText = "lorem ipsum",
                            nodeTypeAlias = "CWS_Home"
                        })
                });


                var searcher = indexer.Searcher;

                var numberSortedCriteria = searcher.CreateQuery()
                    .Field("bodyText", "ipsum");

                var numberSortedResult = numberSortedCriteria
                    .Execute();

                Assert.AreEqual(0.191783011f, numberSortedResult.First().Score);
            }
        }

        [Test]
        public void Score_Freshness_Profile_Out_Of_Range()
        {
            var analyzer = new StandardAnalyzer(LuceneInfo.CurrentVersion);
            using (var luceneDir = new RandomIdRAMDirectory())
            using (var indexer = GetTestIndex(
                luceneDir,
                analyzer,
                new FieldDefinitionCollection(new FieldDefinition("created", "datetime")),
                scoringProfiles: new List<IScoringProfile> { new FreshnessScoringProfile("created", new TimeSpan(1, 0, 0, 0), 1.5f) }))
            {
                indexer.IndexItems(new[]
                {
                    ValueSet.FromObject(123.ToString(), "content",
                        new
                        {
                            created = DateTime.Now.AddDays(-2),
                            bodyText = "lorem ipsum",
                            nodeTypeAlias = "CWS_Home"
                        })
                });


                var searcher = indexer.Searcher;

                var numberSortedCriteria = searcher.CreateQuery()
                    .Field("bodyText", "ipsum");

                var numberSortedResult = numberSortedCriteria
                    .Execute();

                Assert.AreEqual(0.191783011f, numberSortedResult.First().Score);
            }
        }

        [Test]
        public void Score_Freshness_Profile_In_Range()
        {
            var analyzer = new StandardAnalyzer(LuceneInfo.CurrentVersion);
            using (var luceneDir = new RandomIdRAMDirectory())
            using (var indexer = GetTestIndex(
                luceneDir,
                analyzer,
                new FieldDefinitionCollection(new FieldDefinition("created", "datetime")),
                scoringProfiles: new List<IScoringProfile> { new FreshnessScoringProfile("created", new TimeSpan(1, 0, 0, 0), 1.5f) }))
            {
                indexer.IndexItems(new[]
                {
                    ValueSet.FromObject(123.ToString(), "content",
                        new
                        {
                            created = DateTime.Now.AddHours(-5),
                            bodyText = "lorem ipsum",
                            nodeTypeAlias = "CWS_Home"
                        })
                });


                var searcher = indexer.Searcher;

                var numberSortedCriteria = searcher.CreateQuery()
                    .Field("bodyText", "ipsum");

                var numberSortedResult = numberSortedCriteria
                    .Execute();

                Assert.AreEqual(0.287674516f, numberSortedResult.First().Score);
            }
        }

        [Test]
        public void Score_Freshness_Profile_Future_Date()
        {
            var analyzer = new StandardAnalyzer(LuceneInfo.CurrentVersion);
            using (var luceneDir = new RandomIdRAMDirectory())
            using (var indexer = GetTestIndex(
                luceneDir,
                analyzer,
                new FieldDefinitionCollection(new FieldDefinition("created", "datetime")),
                scoringProfiles: new List<IScoringProfile> { new FreshnessScoringProfile("created", new TimeSpan(1, 0, 0, 0), 1.5f) }))
            {
                indexer.IndexItems(new[]
                {
                    ValueSet.FromObject(123.ToString(), "content",
                        new
                        {
                            created = DateTime.Now.AddHours(5),
                            bodyText = "lorem ipsum",
                            nodeTypeAlias = "CWS_Home"
                        })
                });


                var searcher = indexer.Searcher;

                var numberSortedCriteria = searcher.CreateQuery()
                    .Field("bodyText", "ipsum");

                var numberSortedResult = numberSortedCriteria
                    .Execute();

                Assert.AreEqual(0.191783011f, numberSortedResult.First().Score);
            }
        }

        [Test]
        public void Score_Freshness_Profile_Future_Date_With_Future_Duration()
        {
            var analyzer = new StandardAnalyzer(LuceneInfo.CurrentVersion);
            using (var luceneDir = new RandomIdRAMDirectory())
            using (var indexer = GetTestIndex(
                luceneDir,
                analyzer,
                new FieldDefinitionCollection(new FieldDefinition("created", "datetime")),
                scoringProfiles: new List<IScoringProfile> { new FreshnessScoringProfile("created", new TimeSpan(-1, 0, 0, 0), 1.5f) }))
            {
                indexer.IndexItems(new[]
                {
                    ValueSet.FromObject(123.ToString(), "content",
                        new
                        {
                            created = DateTime.Now.AddHours(5),
                            bodyText = "lorem ipsum",
                            nodeTypeAlias = "CWS_Home"
                        })
                });

                var searcher = indexer.Searcher;

                var numberSortedCriteria = searcher.CreateQuery()
                    .Field("bodyText", "ipsum");

                var numberSortedResult = numberSortedCriteria
                    .Execute();

                Assert.AreEqual(0.287674516f, numberSortedResult.First().Score);
            }
        }

        [Test]
        public void Score_Freshness_Profile_Future_Duration()
        {
            var analyzer = new StandardAnalyzer(LuceneInfo.CurrentVersion);
            using (var luceneDir = new RandomIdRAMDirectory())
            using (var indexer = GetTestIndex(
                luceneDir,
                analyzer,
                new FieldDefinitionCollection(new FieldDefinition("created", "datetime")),
                scoringProfiles: new List<IScoringProfile> { new FreshnessScoringProfile("created", new TimeSpan(-1, 0, 0, 0), 1.5f) }))
            {
                indexer.IndexItems(new[]
                {
                    ValueSet.FromObject(123.ToString(), "content",
                        new
                        {
                            created = DateTime.Now.AddDays(-2),
                            bodyText = "lorem ipsum",
                            nodeTypeAlias = "CWS_Home"
                        })
                });

                var searcher = indexer.Searcher;

                var numberSortedCriteria = searcher.CreateQuery()
                    .Field("bodyText", "ipsum");

                var numberSortedResult = numberSortedCriteria
                    .Execute();

                Assert.AreEqual(0.191783011f, numberSortedResult.First().Score);
            }
        }

        [Test]
        public void Score_Freshness_Add_On_Searcher()
        {
            var analyzer = new StandardAnalyzer(LuceneInfo.CurrentVersion);
            using (var luceneDir = new RandomIdRAMDirectory())
            using (var indexer = GetTestIndex(
                luceneDir,
                analyzer,
                new FieldDefinitionCollection(new FieldDefinition("created", "datetime"))))
            {
                indexer.IndexItems(new[]
                {
                ValueSet.FromObject(123.ToString(), "content",
                    new
                    {
                        created = DateTime.Now.AddDays(-2),
                        bodyText = "lorem ipsum",
                        nodeTypeAlias = "CWS_Home"
                    })
            });

                var searcher = indexer.Searcher;

                searcher.AddScoringProfile(new FreshnessScoringProfile("created", new TimeSpan(-1, 0, 0, 0), 1.5f));

                var numberSortedCriteria = searcher.CreateQuery()
                    .Field("bodyText", "ipsum");

                var numberSortedResult = numberSortedCriteria
                    .Execute();

                Assert.AreEqual(0.191783011f, numberSortedResult.First().Score);
            }
        }
    }
}