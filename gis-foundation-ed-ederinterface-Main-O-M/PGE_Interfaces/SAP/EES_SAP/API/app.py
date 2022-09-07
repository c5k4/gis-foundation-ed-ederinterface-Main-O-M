from flask import Flask, request

import GISSAP
import json


#app.config['MONGO_DBNAME'] = conf.MONGO_DATABASE_NAME
#app.config['MONGO_URI'] = conf.MONGO_DATABASE_URI
#mongo = PyMongo(app, replicaset="mongors") if conf.MONGO_REPLICASET else PyMongo(app)

app = Flask(__name__)


@app.route('/')
def home():
    """
    This function is just for testing api
    :return: This function will return the text message
    """
    return "GRC PROD Homepage"

@app.errorhandler(405)
def method_not_allowed(e):
    """
    This function will executed if any api called other than GET and Post http methods
    :param e:
    :return: Json response status code 405
    """
    return jsonify(error=405, text=str(e)), 405

@app.route('/SAPGIS/service/data/', methods=['GET'])
def report_data():
    """
    This will call the GIS and Landbase data.
    Request method: GET
    Request params:PM Order Number and System Type
    :return: will get the grc summary data in json format
    """
    try:
        if 'OrderNo' and 'System' in request.args:
            orderNum = request.args['OrderNo']
            systemTy = request.args['System']
        else:
            return "Provide valid data"
        response_data = {}
        
        response_data["GISdata"] = GISSAP.get_GIS_Globals(orderNum, systemTy)
        #response_data["status"] = "SUCCESS"
        #response_data["message"] = "Data Generated Successfully"
        #response_data["data"] = report_data
        return json.dumps(response_data)
    except Exception as e:
        return "Error in executing the Service"+ e

if __name__ == "__main__":
    app.run(host= '0.0.0.0',port=5000,debug=True)
