using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.UI;
using System.IO;
using Amazon;
using Amazon.S3;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.S3.Model;

namespace program4
{
    public partial class _Default : Page
    {
        private const string sourceBucket = "css490", myBucket = "poppyla-program4";
        private const string inputFileName = "input.txt", myFileName = "pg4data.txt";
        private static List<string> dataList = new List<string>();
        private static string myTable = "DataList";
        private static string result = "";

        private static readonly RegionEndpoint myRegion = RegionEndpoint.USWest2;
        private static IAmazonS3 s3Client;
        private static AmazonDynamoDBClient ddbClient;

        protected void Page_Load(object sender, EventArgs e)
        {
            s3Client = new AmazonS3Client(myRegion);
            ddbClient = new AmazonDynamoDBClient(myRegion);
            Button3.Enabled = false;        // prevent delete empty object and db
        }

        // Load button click
        protected void Button1_Click(object sender, EventArgs e)
        {

            //copying object from source URL
            try
            {
                CopyObjectRequest req = new CopyObjectRequest
                {
                    SourceBucket = sourceBucket,
                    DestinationBucket = myBucket,
                    SourceKey = inputFileName,
                    DestinationKey = myFileName,
                    CannedACL= S3CannedACL.PublicRead       // set the object public
                };
                CopyObjectResponse res = s3Client.CopyObject(req);

                // create new DataList table if table not exist
                if (!TableExists())
                {
                    CreateTable().Wait();
                }
                // load data into db table
                DownloadList().Wait();
                ParseData().Wait();
                Label1.Text = "Data loaded successfully!";      // label 1 shows load result 
                Label2.Text = " ";                              // label 2 shows query result
                Label3.Text = " ";                              // label 3 shows delete result
                Button3.Enabled = true;
            }
            catch (Exception exception)
            {
                Console.WriteLine("LOAD DATA ERROR:", exception.Message);
            }
        }

        // check if table exist 
        private static bool TableExists()
        {
            DescribeTableRequest request = new DescribeTableRequest
            {
                TableName = myTable
            };

            // table description will be returned if DataList table is exist
            try
            {
                TableDescription description = ddbClient.DescribeTable(request).Table;
                return true;
            }
            catch (ResourceNotFoundException)
            {
                return false;
            }
        }

        // create table in dynamodb
        private static Task CreateTable()
        {
            var res = ddbClient.CreateTable(new CreateTableRequest
            {
                TableName = myTable,
                AttributeDefinitions = new List<AttributeDefinition>()
                {
                    new AttributeDefinition { AttributeName = "LastName", AttributeType = "S" },
                    new AttributeDefinition { AttributeName = "FirstName", AttributeType = "S" }
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement { AttributeName = "LastName", KeyType = "HASH" },
                    new KeySchemaElement { AttributeName = "FirstName", KeyType = "RANGE" }
                },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 15,
                    WriteCapacityUnits = 15
                },
            });

            // wait until table created, check status of describe table
            string status = res.TableDescription.TableStatus;
            while (status != "ACTIVE")
            {
                System.Threading.Thread.Sleep(1000); // Wait 1 seconds.
                try
                {
                    var respond = ddbClient.DescribeTable(new DescribeTableRequest
                    {
                        TableName = myTable
                    });

                    status = respond.Table.TableStatus;
                }
                catch (ResourceNotFoundException ex)
                {
                    Console.WriteLine("CREATE TABLE ERROR: ", ex);
                }
            }
            return Task.CompletedTask;
        }

        // download list from the source to s3 object
        private static Task DownloadList()
        {
            WebClient client = new WebClient();
            string data = client.DownloadString("https://s3-us-west-2.amazonaws.com/css490/input.txt");

            StringReader reader = new StringReader(data);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                dataList.Add(line);
            }
            return Task.CompletedTask;
        }

        // add data to table
        private static Task ParseData()
        {
            // go through datalist
            for (int i = 0; i < dataList.Count; i++)
            {
                Dictionary<string, AttributeValue> person = new Dictionary<string, AttributeValue>();

                // get first and last name
                string[] attributes = dataList[i].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                // empty file/empty line
                if (attributes.Length < 1)
                {
                    continue;
                }
                // prevent last name only
                person.Add("LastName", new AttributeValue { S = attributes[0] });
                if (attributes.Length >= 2)
                {
                    person.Add("FirstName", new AttributeValue { S = attributes[1] });
                    for (int j = 2; j < attributes.Length; j++)
                    {
                        string[] attElements = attributes[j].Split('=');
                        person.Add(attElements[0], new AttributeValue { S = attElements[1] });
                    }  
                } 
                else
                {
                    person.Add("FirstName", new AttributeValue { S = " " });
                }

                // create request to put item in table
                PutItemRequest requestItem = new PutItemRequest
                {
                    TableName = myTable,
                    Item = person
                };

                // add the put item request, this will add new person to the list, or update their info if they're existed
                ddbClient.PutItem(requestItem);
                person.Clear();
            }
            return Task.CompletedTask;
        }


        // query button
        protected void Button2_Click(object sender, EventArgs e)
        {
            string lastName = TextBox2.Text;
            string firstName = TextBox1.Text;
            QueryName(lastName, firstName).Wait();

            // check if result is empty
            if (String.IsNullOrEmpty(result))
            {
                Label2.Text = "Name not found or empty database.";
            }
            else
            {
                Label2.Text = result.Replace("\r\n", "<br />");
                Button3.Enabled = true;        // table exist, allow user to click clear button
            }
            result = "";
            Label1.Text = Label3.Text = " ";
        }

        // query the request name
        private static Task QueryName(string lastName, string firstName)
        {
            AmazonDynamoDBClient myDB = new AmazonDynamoDBClient(myRegion);
            ScanRequest request = new ScanRequest
            {
                TableName = myTable
            };
            try
            {   
                // db scan to check the requested name
                var response = myDB.Scan(request);
                string itemFirst, itemLast;

                // check every item  in respond with match name request
                foreach (Dictionary<string, AttributeValue> item in response.Items)
                {
                    itemFirst = item["FirstName"].S;        // convert lastname/firstname in item
                    itemLast = item["LastName"].S;          // to string
                    //if user inputs both first and last name
                    if (!string.IsNullOrEmpty(lastName) && !string.IsNullOrEmpty(firstName))
                    {
                        if (itemLast.Equals(lastName, StringComparison.OrdinalIgnoreCase) && itemFirst.Equals(firstName, StringComparison.OrdinalIgnoreCase))
                        {
                            result += "LastName=" + itemLast + "  FirstName=" + itemFirst + "\r\n";
                            getAttributes(item);
                        }
                    }
                    // if user only inputs last name.
                    else if (!string.IsNullOrEmpty(lastName))
                    {
                        if (itemLast.Equals(lastName, StringComparison.OrdinalIgnoreCase))
                        {
                            result += "LastName=" + itemLast + "  FirstName=" + itemFirst + "\r\n";
                            getAttributes(item);
                        }
                    }
                    // if user only inputs first name.
                    else if (!string.IsNullOrEmpty(firstName))
                    {
                        if (itemFirst.Equals(firstName, StringComparison.OrdinalIgnoreCase))
                        {
                            result += "LastName=" + itemLast + "   FirstName=" + itemFirst + "\r\n";
                            getAttributes(item);
                        }
                    }
                    else
                    {
                        return Task.CompletedTask;
                    }
                }
            }
            catch (ResourceNotFoundException ex)
            {
                Console.WriteLine("QUERRY ERROR: ", ex);
            }
            return Task.CompletedTask;
        }

        // get the person's attribute values
        private static Task getAttributes(Dictionary<string, AttributeValue> item)
        {
            foreach (KeyValuePair<string, AttributeValue> pair in item)
            {
                if (pair.Key != "LastName" && pair.Key != "FirstName")
                {
                    if (pair.Value.S == null)
                    {
                        result += pair.Key + "\r\n";            // empty attribute
                    }
                    else
                    {
                        result += pair.Key + ": " + pair.Value.S + "\r\n";
                    }
                }
            }
            result += "\r\n\r\n";
            return Task.CompletedTask;
        }


        // clear data button
        protected void Button3_Click(object sender, EventArgs e)
        {
            // prevent double click
            Button3.Enabled = false;

            DeleteTableRequest tableReq = new DeleteTableRequest
            {
                TableName = myTable
            };

            DeleteObjectRequest objReq = new DeleteObjectRequest
            {
                BucketName = myBucket,
                Key = myFileName
            };

            try
            {
                var response = ddbClient.DeleteTable(tableReq);         // delete table in dynamoDB
                s3Client.DeleteObject(objReq);                          // delete object in s3

            }
            catch (ResourceNotFoundException ex)
            {
                Console.WriteLine("CLEAR DATA ERROR: ", ex);
            }
            Label1.Text = " ";
            Label2.Text = " ";
            Label3.Text = "Data has been cleared!";
            TextBox1.Text = "";
            TextBox2.Text = "";
        }

        protected void TextBox1_TextChanged(object sender, EventArgs e)
        {
            Label1.Text = " ";
            Label3.Text = " ";
        }

        protected void TextBox2_TextChanged(object sender, EventArgs e)
        {
            Label1.Text = " ";
            Label3.Text = " ";
        }
    }
}