import sqlite3
import pandas as pd
from sklearn.tree import DecisionTreeClassifier
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import OneHotEncoder
from sklearn.compose import ColumnTransformer
from sklearn.pipeline import make_pipeline

# Connect to the SQLite database
conn = sqlite3.connect('Bookstore.sqlite')

# Load data into a pandas DataFrame
query = "SELECT * FROM Book"
df = pd.read_sql_query(query, conn)
conn.close()

# Assuming we are predicting 'Category' based on other features
y = df['Category']
X = df.drop(columns=['BookId', 'Title', 'Category'])  # Exclude unique identifiers and the target

# Encode categorical variables
categorical_features = ['Author', 'Publisher', 'Isbn', 'Classification']
numeric_features = ['PageCount', 'Price']

# Preprocessing pipeline
preprocessor = ColumnTransformer(
    transformers=[
        ('num', 'passthrough', numeric_features),
        ('cat', OneHotEncoder(), categorical_features)
    ])

# Create a pipeline that encodes categories, then fits a DecisionTreeClassifier
pipeline = make_pipeline(preprocessor, DecisionTreeClassifier(max_depth=5))

# Split the data
X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=.3, random_state=42)

# Train the model
model = pipeline.fit(X_train, y_train)

# Evaluate the model
print(f'Accuracy:\t{model.score(X_test, y_test)}')

# Converting the model to ONNX format is similar, ensure you get the correct input feature shape after preprocessing
# This step may require adjusting the initial_type to match the transformed feature array shape
