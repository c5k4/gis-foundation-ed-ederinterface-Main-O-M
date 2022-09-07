import json

import cx_Oracle
import myLogger



class DatabaseConnection:

    # Function to connect to Oralce DB
    def __init__(self, username, password, connection_name):
        
        try:
            
            self.oracle_connection = cx_Oracle.connect(username,password,connection_name)
            
            
            print "Connected"
        except Exception as exc:
            print ("unable to connect to Oracle DB" ,exc)
            #self.search_logger.writeTransactionLog(self.transaction_name, self.transaction_input, 'DatabaseConnection()', 'Failed', str(exc), 'error', False, False)

    def close_connection(self):

        try:
            self.oracle_connection.close()
        except Exception as exc:
            print (exc)
            #self.search_logger.writeTransactionLog(self.transaction_name, self.transaction_input, 'close_connection()', 'Failed', str(exc), 'error', False, False)

    def CreateProcedureParameter(self, cursor, division_name, search_type, operator_type, search_text, search_procedure_dictionary, procedure_parameter_count):

        try:
            # Initializing "parameter_item_list" to add the procedure parameters and return
            parameter_item_list = []
            procedure_name = ''

            # Converting dictionary and list string to a python object
            procedure_dictionary = json.loads(search_procedure_dictionary)
            parameter_count_list = json.loads(procedure_parameter_count)        

            # Initializing "procedure_dictionary_item_count" to iterate through the dictionary
            procedure_dictionary_item_count = 0
            # Searching through the dictionary for the search type and it's corresponding store procedure name
            for key,value in procedure_dictionary.items():
                if key == search_type:
                    procedure_name = value
                    parameter_count = parameter_count_list[procedure_dictionary_item_count]
                procedure_dictionary_item_count = procedure_dictionary_item_count + 1

            # Buiding up store procedure parameter list
            parameter_list = [search_text, division_name, operator_type]

            # Creation of cursor variable object during runtime depending on the store procedure parameters
            # Append the variable created to the parameter_list
            x = 1
            while x <= parameter_count:
                output_argument = cursor.var(cx_Oracle.CURSOR)
                parameter_list.append(output_argument)
                x = x + 1

            parameter_item_list.append(procedure_name)
            parameter_item_list.append(parameter_list)

            return parameter_item_list


        except Exception as exc:
            print str(exc)
            #self.search_logger.writeTransactionLog(self.transaction_name, self.transaction_input, 'CreateProcedureParameter()', 'Failed', str(exc), 'error', False, False)
