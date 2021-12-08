# QueryWebApp
Create and host a Website which has three sections: A button call Load Data, a button call Clear Data, A button called Query with 2 input text boxes labeled First Name and Last Name. Main code is stored in Default.aspx.cs.

Uses:
- AWS S3 as data storage
- AWS DynamoDB to create NoSQL table
- AWS Elastic Beanstalk to deploy the web application

When the “Load Data” button is hit the website will load data from an object stored at a given 
URL: https://s3-us-west-2.amazonaws.com/css490/input.txt. The CORS (Cross-Origin Resource
Sharing) header has been added to the bucket so that the object can be accessed from different
regions.  

When the “Clear Data” button is hit the blob is removed from the object store. The NoSQL 
table is also emptied or removed.

Once the data has been loaded in the NoSQL store the Website user can type in either one or 
both a First and Last name.  When the Query button is hit results are shown on the Website.  
For the queries the names should be exact matches.

