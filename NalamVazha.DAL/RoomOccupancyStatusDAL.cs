namespace NalamVazha.DAL{
			    using System;
			    using System.Text;
			    using System.Data;
			    using System.Data.Common;
			    using NalamVazha.Models;
			    using EncrypDecrypt;
			    using Newtonsoft.Json;
				using Newtonsoft.Json.Linq;
                using Npgsql;
				using NpgsqlTypes;
				using System.Text.RegularExpressions;

			    //This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:04
			    public class RoomOccupancyStatusDAL
			    {
					public virtual string db_connectionstring{get;set;}
					
			 	    public RoomOccupancyStatusDAL(string connectionString)
				    {
						
					    db_connectionstring=connectionString;
				    }
				  
			        
              public virtual string Add_Room_Occupancy_Status(RoomOccupancyStatusModel model)
			  { 
				  String ResponseMessage="";
					 
					try{
							 
                            using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					        {
						        npsql.Open();
						        using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Add_Room_Occupancy_Status\"(@pvar_roomoccupancystatusid,@pvar_tenantid,@pvar_roomallocationno,@pvar_patientvisit,@pvar_patientname,@pvar_ipdno,@pvar_block,@pvar_building,@pvar_floor,@pvar_room,@pvar_bookeddate,@pvar_status,@pvar_createduser)", npsql))
						        {
                                        dbCommand.CommandType = CommandType.Text;
						            	
								        					dbCommand.Parameters.AddWithValue("pvar_roomoccupancystatusid",NpgsqlDbType.Uuid,(object)model.RoomOccupancyStatusid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)model.tenantid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_roomallocationno",NpgsqlDbType.Varchar,(object)model.roomallocationno??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_patientvisit",NpgsqlDbType.Uuid,(object)model.patientvisit??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_patientname",NpgsqlDbType.Uuid,(object)model.patientname??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_ipdno",NpgsqlDbType.Uuid,(object)model.ipdno??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_block",NpgsqlDbType.Uuid,(object)model.block??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_building",NpgsqlDbType.Uuid,(object)model.building??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_floor",NpgsqlDbType.Uuid,(object)model.floor??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_room",NpgsqlDbType.Uuid,(object)model.room??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_bookeddate",NpgsqlDbType.Date,(object)model.bookeddate??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_status",NpgsqlDbType.Varchar,(object)model.status??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_createduser",NpgsqlDbType.Uuid,(object)model.createduser??DBNull.Value);	
															
										
                                        NpgsqlParameter outParm = new NpgsqlParameter("pvar_returnMessage", NpgsqlDbType.Varchar)
                                        {
                                             Direction = ParameterDirection.Output
                                        };
                                        dbCommand.Parameters.Add(outParm);

                                        dbCommand.ExecuteNonQuery();
								        ResponseMessage = outParm.Value.ToString();
								        if (dbCommand.Connection.State != ConnectionState.Closed)
                    			        {
										         dbCommand.Connection.Dispose();
								        }

						        }
						        npsql.Close();
					        }
 

					}catch(Exception ex){
						ResponseMessage=ex.Message;
						Console.WriteLine(ex);
					} 
					
					return ResponseMessage;

			   }
public virtual RoomOccupancyStatusModel getById_RoomOccupancyStatus(string RoomOccupancyStatusid)
									 {
										DataTable dataTable = new DataTable();
										DataSet dataSet = new DataSet();
										try{
												 
												using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
												{
													npsql.Open();
													using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_RoomOccupancyStatus\"(@pvar_roomoccupancystatusid)", npsql))
													{
														dbCommand.CommandType = CommandType.Text;
														dbCommand.Parameters.AddWithValue("pvar_roomoccupancystatusid",(object)RoomOccupancyStatusid??DBNull.Value);
														using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbCommand))
														{
															dataSet.Reset();
															dataAdapter.Fill(dataSet);
															dataTable = dataSet.Tables[0];
															if (dbCommand.Connection.State != ConnectionState.Closed)
															{
																dbCommand.Connection.Dispose();
															}
														}
													}
													npsql.Close();
												}
					 
										}catch{
												throw;
										}
										if (dataTable.Rows.Count > 0)
										{
											DataRow row = dataTable.Rows[0];
											return ModelConverter.ConvertDataRowToModel<RoomOccupancyStatusModel>(row);
										}
										else
										{
											return null;
										}
									 }
			 public virtual string  Update_Room_Occupancy_Status(RoomOccupancyStatusModel model)
			 { 
				 String ResponseMessage="";
					try{
						 	 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Update_Room_Occupancy_Status\"(@pvar_roomoccupancystatusid,@pvar_tenantid,@pvar_roomallocationno,@pvar_patientvisit,@pvar_patientname,@pvar_ipdno,@pvar_block,@pvar_building,@pvar_floor,@pvar_room,@pvar_bookeddate,@pvar_status,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
															dbCommand.Parameters.AddWithValue("pvar_roomoccupancystatusid",NpgsqlDbType.Uuid,(object)model.RoomOccupancyStatusid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)model.tenantid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_roomallocationno",NpgsqlDbType.Varchar,(object)model.roomallocationno??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_patientvisit",NpgsqlDbType.Uuid,(object)model.patientvisit??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_patientname",NpgsqlDbType.Uuid,(object)model.patientname??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_ipdno",NpgsqlDbType.Uuid,(object)model.ipdno??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_block",NpgsqlDbType.Uuid,(object)model.block??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_building",NpgsqlDbType.Uuid,(object)model.building??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_floor",NpgsqlDbType.Uuid,(object)model.floor??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_room",NpgsqlDbType.Uuid,(object)model.room??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_bookeddate",NpgsqlDbType.Date,(object)model.bookeddate??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_status",NpgsqlDbType.Varchar,(object)model.status??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_modifieduser",NpgsqlDbType.Uuid,model.modifieduser);	
															
										NpgsqlParameter outParm = new NpgsqlParameter("@returnMessage", NpgsqlDbType.Varchar)
										{
											 Direction = ParameterDirection.Output
										};
										dbCommand.Parameters.Add(outParm);

										dbCommand.ExecuteNonQuery();
										ResponseMessage = outParm.Value.ToString();
										if (dbCommand.Connection.State != ConnectionState.Closed)
										{
												 dbCommand.Connection.Dispose();
										}

								}
								npsql.Close();
							}		 

					}catch(Exception ex){
						ResponseMessage=ex.Message;
					}
					
					return ResponseMessage;

			   }
		/// <summary>
		/// Bulk-updates all RoomOccupancyStatus records for a given IPD (ipdno) from "Blocked" to the supplied newstatus (e.g. "Admitted").
		/// Uses a direct SQL UPDATE to avoid the need for a separate stored procedure per record.
		/// </summary>
		public virtual string Update_Room_Occupancy_By_IPD(string ipdno, string newstatus, string modifieduser)
		{
			string responseMessage = "";
			try
			{
				using (var npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var cmd = new NpgsqlCommand(
						"SELECT * FROM \"Update_Room_Occupancy_By_IPD\"(@pvar_ipdno, @pvar_newstatus, @pvar_modifieduser)", npsql))
					{
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.AddWithValue("pvar_ipdno", NpgsqlDbType.Uuid,
							Guid.TryParse(ipdno, out var parsedIpd) ? (object)parsedIpd : DBNull.Value);
						cmd.Parameters.AddWithValue("pvar_newstatus", NpgsqlDbType.Varchar, (object)newstatus ?? DBNull.Value);
						cmd.Parameters.AddWithValue("pvar_modifieduser", NpgsqlDbType.Uuid,
							Guid.TryParse(modifieduser, out var parsedUser) ? (object)parsedUser : DBNull.Value);
						NpgsqlParameter outParm = new NpgsqlParameter("pvar_returnMessage", NpgsqlDbType.Varchar)
						{
							Direction = ParameterDirection.Output
						};
						cmd.Parameters.Add(outParm);
						cmd.ExecuteNonQuery();
						responseMessage = outParm.Value?.ToString() ?? "";
					}
					npsql.Close();
				}
			}
			catch (Exception ex)
			{
				responseMessage = ex.Message;
			}
			return responseMessage;
		}


		public virtual string Release_Room_On_Cancellation(string ipdno, string modifieduser)
		{
			string responseMessage = "";
			try
			{
				using (var npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var cmd = new NpgsqlCommand(
						"SELECT * FROM \"Release_Room_On_Cancellation\"(@pvar_ipdno, @pvar_modifieduser)", npsql))
					{
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.AddWithValue("pvar_ipdno", NpgsqlDbType.Uuid,
							Guid.TryParse(ipdno, out var parsedIpd) ? (object)parsedIpd : DBNull.Value);
						cmd.Parameters.AddWithValue("pvar_modifieduser", NpgsqlDbType.Uuid,
							Guid.TryParse(modifieduser, out var parsedUser) ? (object)parsedUser : DBNull.Value);
						NpgsqlParameter outParm = new NpgsqlParameter("pvar_returnMessage", NpgsqlDbType.Varchar)
						{
							Direction = ParameterDirection.Output
						};
						cmd.Parameters.Add(outParm);
						cmd.ExecuteNonQuery();
						responseMessage = outParm.Value?.ToString() ?? "";
					}
					npsql.Close();
				}
			}
			catch (Exception ex)
			{
				responseMessage = ex.Message;
			}
			return responseMessage;
		}

		/// <summary>
		/// Releases the room(s) allocated to an IPD booking on discharge.
		/// Sets status = 'Vacant' on all active RoomOccupancyStatus records for the given IPD,
		/// keeping the history record intact (isdeleted remains false).
		/// </summary>
		public virtual string Release_Rooms_On_Discharge(string ipdno, string modifieduser)
		{
			string responseMessage = "";
			try
			{
				using (var npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var cmd = new NpgsqlCommand("SELECT release_rooms_on_discharge(@pipdno, @pmodifieduser)", npsql))
					{
						cmd.Parameters.AddWithValue("pipdno", NpgsqlDbType.Uuid,
							Guid.TryParse(ipdno, out var parsedIpd) ? (object)parsedIpd : DBNull.Value);
						cmd.Parameters.AddWithValue("pmodifieduser", NpgsqlDbType.Uuid,
							Guid.TryParse(modifieduser, out var parsedUser) ? (object)parsedUser : DBNull.Value);

						responseMessage = cmd.ExecuteScalar()?.ToString() ?? "";
					}
					npsql.Close();
				}
			}
			catch (Exception ex)
			{
				responseMessage = ex.Message;
			}
			return responseMessage;
		}

        public virtual string Insert_RoomAllocation_And_Occupancy_For_Transfer(
    RoomOccupancyStatusModel model,
    DateTime fromDate,
    DateTime toDate,
    string allottedTo
)
        {
            string responseMessage = "";

            try
            {
                if (fromDate.Date > toDate.Date)
                    return "From date cannot be greater than To date.";

                string allocationPrefix = !string.IsNullOrWhiteSpace(model.roomallocationno)
                    ? model.roomallocationno
                    : "OCC";

                string roomAllocationNo = $"{allocationPrefix}-{fromDate:yyyyMMdd}-{Guid.NewGuid():N}";

                if (roomAllocationNo.Length > 128)
                    roomAllocationNo = roomAllocationNo.Substring(0, 128);

                using (var npsql = new NpgsqlConnection(db_connectionstring))
                {
                    npsql.Open();

                    using (var cmd = new NpgsqlCommand(
                        @"SELECT public.""Insert_RoomAllocation_And_Occupancy_For_Transfer""
                (
                    @pvar_tenantid,
                    @pvar_roomallocationno,
                    @pvar_patientvisit,
                    @pvar_patientname,
                    @pvar_ipdno,

                    @pvar_block,
                    @pvar_building,
                    @pvar_floor,
                    @pvar_room,

                    @pvar_fromdate,
                    @pvar_todate,
                    @pvar_allottedto,

                    @pvar_status,
                    @pvar_createduser
                );", npsql))
                    {
                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.AddWithValue("pvar_tenantid", NpgsqlDbType.Uuid, (object)model.tenantid ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("pvar_roomallocationno", NpgsqlDbType.Varchar, roomAllocationNo);
                        cmd.Parameters.AddWithValue("pvar_patientvisit", NpgsqlDbType.Uuid, (object)model.patientvisit ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("pvar_patientname", NpgsqlDbType.Uuid, (object)model.patientname ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("pvar_ipdno", NpgsqlDbType.Uuid, model.ipdno);

                        cmd.Parameters.AddWithValue("pvar_block", NpgsqlDbType.Uuid, model.block);
                        cmd.Parameters.AddWithValue("pvar_building", NpgsqlDbType.Uuid, model.building);
                        cmd.Parameters.AddWithValue("pvar_floor", NpgsqlDbType.Uuid, model.floor);
                        cmd.Parameters.AddWithValue("pvar_room", NpgsqlDbType.Uuid, model.room);

                        cmd.Parameters.AddWithValue("pvar_fromdate", NpgsqlDbType.Date, fromDate.Date);
                        cmd.Parameters.AddWithValue("pvar_todate", NpgsqlDbType.Date, toDate.Date);
                        cmd.Parameters.AddWithValue("pvar_allottedto", NpgsqlDbType.Varchar, allottedTo ?? "");

                        cmd.Parameters.AddWithValue("pvar_status", NpgsqlDbType.Varchar, string.IsNullOrWhiteSpace(model.status) ? "Occupied" : model.status);
                        cmd.Parameters.AddWithValue("pvar_createduser", NpgsqlDbType.Uuid, (object)model.createduser ?? DBNull.Value);

                        responseMessage = cmd.ExecuteScalar()?.ToString() ?? "";
                    }
                }
            }
            catch (Exception ex)
            {
                responseMessage = ex.Message;
            }

            return responseMessage;
        }

        public virtual string Release_Room_Occupancy_For_Transfer(
    Guid ipdno,
    DateTime fromDate,
    DateTime toDate,
    Guid modifieduser,
    string allottedTo
)
        {
            string responseMessage = "";

            try
            {
                using (var npsql = new NpgsqlConnection(db_connectionstring))
                {
                    npsql.Open();

                    using (var cmd = new NpgsqlCommand(
                        @"SELECT public.""Release_Room_Occupancy_For_Transfer""
                (
                    @pvar_ipdno,
                    @pvar_fromdate,
                    @pvar_modifieduser,
                    @pvar_todate,
                    @pvar_allottedto
                );", npsql))
                    {
                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.AddWithValue("pvar_ipdno", NpgsqlDbType.Uuid, ipdno);
                        cmd.Parameters.AddWithValue("pvar_fromdate", NpgsqlDbType.Date, fromDate.Date);
                        cmd.Parameters.AddWithValue("pvar_modifieduser", NpgsqlDbType.Uuid, modifieduser);
                        cmd.Parameters.AddWithValue("pvar_todate", NpgsqlDbType.Date, toDate.Date);
                        cmd.Parameters.AddWithValue(
                            "pvar_allottedto",
                            NpgsqlDbType.Varchar,
                            string.IsNullOrWhiteSpace(allottedTo) ? (object)DBNull.Value : allottedTo
                        );

                        responseMessage = cmd.ExecuteScalar()?.ToString() ?? "";
                    }
                }
            }
            catch (Exception ex)
            {
                responseMessage = ex.Message;
            }

            return responseMessage;
        }
        public virtual string  Remove_Room_Occupancy_Status(string id,string loginUserID)
			  { 
				  String ResponseMessage="";
					try{ 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Remove_Room_Occupancy_Status\"(@pvar_roomoccupancystatusid,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
										dbCommand.Parameters.AddWithValue("pvar_roomoccupancystatusid",(object)id??DBNull.Value);
										dbCommand.Parameters.AddWithValue("pvar_modifieduser",(object)loginUserID??DBNull.Value);
										NpgsqlParameter outParm = new NpgsqlParameter("@returnMessage", NpgsqlDbType.Varchar)
										{
											 Direction = ParameterDirection.Output
										};
										dbCommand.Parameters.Add(outParm);

										dbCommand.ExecuteNonQuery();
										ResponseMessage = outParm.Value.ToString();
										if (dbCommand.Connection.State != ConnectionState.Closed)
										{
												 dbCommand.Connection.Dispose();
										}

								}
								npsql.Close();
							}	 

					}catch(Exception ex){
						ResponseMessage=ex.Message;
					}
					
					return ResponseMessage;

			   }
public virtual JObject Room_Occupancy_Status_List(string tenantid
,string patientvisit
,string patientname
,string ipdno
, int? pagesize=1000 , int? pagenumber=0,string searchterm="",string  sort_fields="")
			  { 
				  object dalResponse = null;
			
					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM\"Room_Occupancy_Status_List\"(@pvar_tenantid,@pvar_patientvisit,@pvar_patientname,@pvar_ipdno,@pvar_pagesize,@pvar_pagenumber,@pvar_searchterm,@pvar_sort_fields)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_patientvisit",(object)patientvisit??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_patientname",(object)patientname??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_ipdno",(object)ipdno??DBNull.Value);

									dbCommand.Parameters.AddWithValue("pvar_pagesize",(object)pagesize??DBNull.Value);
									dbCommand.Parameters.AddWithValue("pvar_pagenumber",(object)pagenumber??DBNull.Value);

									dbCommand.Parameters.AddWithValue("pvar_searchterm",(object)searchterm??DBNull.Value);
									if (sort_fields != null && sort_fields.Length > 2)
									dbCommand.Parameters.AddWithValue("pvar_sort_fields", NpgsqlDbType.Json, sort_fields);
									else
									dbCommand.Parameters.AddWithValue("pvar_sort_fields", NpgsqlDbType.Json, DBNull.Value);


									npsql.Open();
									dalResponse = dbCommand.ExecuteScalar();
									npsql.Close();
									
								}
								
							}

						 

					}catch{
						throw;
					}


					return JObject.Parse(dalResponse.ToString());	


			   }
			   
			 
public virtual System.Data.DataTable get_all_RoomOccupancyStatus(string tenantid,string searchterm="", int? pagesize=1000, int? pagenumber=0)
			  { 

				    DataTable dataTable = new DataTable();
					DataSet dataSet = new DataSet();

					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"get_all_RoomOccupancyStatus\"(@pvar_tenantid)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);
									dbCommand.Parameters.AddWithValue("pvar_searchterm", (object)searchterm?? DBNull.Value);
                dbCommand.Parameters.AddWithValue("pvar_pagesize", (object)pagesize ?? DBNull.Value);
                dbCommand.Parameters.AddWithValue("pvar_pagenumber", (object)pagenumber ?? DBNull.Value);
									using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbCommand))
									{
										dataSet.Reset();
										dataAdapter.Fill(dataSet);
										dataTable = dataSet.Tables[0];
										if (dbCommand.Connection.State != ConnectionState.Closed)
										{
											dbCommand.Connection.Dispose();
										}
									}
								}
								npsql.Close();
							}
						

					}catch{
						throw;
					}
					return dataTable;	


					 

			   }
public virtual System.Data.DataTable getById_allinfo_RoomOccupancyStatus(string RoomOccupancyStatusid)
			 {
				DataSet dataSet=new DataSet();
				DataTable dataTable = new DataTable();
				try{
					     
						using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
						{
							npsql.Open();
							using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_all_RoomOccupancyStatus\"(@pvar_roomoccupancystatusid)", npsql))
							{
								dbCommand.CommandType = CommandType.Text;
								dbCommand.Parameters.AddWithValue("pvar_roomoccupancystatusid",(object)RoomOccupancyStatusid??DBNull.Value);
								using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbCommand))
								{
									dataSet.Reset();
									dataAdapter.Fill(dataSet);
									dataTable = dataSet.Tables[0];
									if (dbCommand.Connection.State != ConnectionState.Closed)
									{
										dbCommand.Connection.Dispose();
									}
								}
							}
							npsql.Close();
						}
					 
				}catch{
						throw;
				}
				return dataTable;
			 }
			  
public virtual System.Data.DataTable lookup_RoomOccupancyStatus_patientvisit(String tenantid,string searchterm="", int? pagesize=1000, int? pagenumber=0)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_RoomOccupancyStatus_patientvisit\"(@pvar_tenantid,@pvar_searchterm,@pvar_pagesize,@pvar_pagenumber)", npsql))
						                                    {
                                            
                                                                dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);  
																dbCommand.Parameters.AddWithValue("pvar_searchterm", (object)searchterm?? DBNull.Value);
                dbCommand.Parameters.AddWithValue("pvar_pagesize", (object)pagesize ?? DBNull.Value);
                dbCommand.Parameters.AddWithValue("pvar_pagenumber", (object)pagenumber ?? DBNull.Value);
																 
                                                                dbCommand.CommandType = CommandType.Text;
							                                    using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbCommand))
                                                                {
                                                                    dataSet.Reset();
                                                                    dataAdapter.Fill(dataSet);
                                                                    dataTable = dataSet.Tables[0];
                                                                    if (dbCommand.Connection.State != ConnectionState.Closed)
												                    {
													                    dbCommand.Connection.Dispose();
												                    }
                                                                }
						                                    }
						                                    npsql.Close();
					                                    }
			 

										 
									        }catch{
										        throw;
									        }
									        return dataTable;
							        }
public virtual System.Data.DataTable lookup_RoomOccupancyStatus_patientname(String tenantid,string searchterm="", int? pagesize=1000, int? pagenumber=0)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_RoomOccupancyStatus_patientname\"(@pvar_tenantid,@pvar_searchterm,@pvar_pagesize,@pvar_pagenumber)", npsql))
						                                    {
                                            
                                                                dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);  
																dbCommand.Parameters.AddWithValue("pvar_searchterm", (object)searchterm?? DBNull.Value);
                dbCommand.Parameters.AddWithValue("pvar_pagesize", (object)pagesize ?? DBNull.Value);
                dbCommand.Parameters.AddWithValue("pvar_pagenumber", (object)pagenumber ?? DBNull.Value);
																 
                                                                dbCommand.CommandType = CommandType.Text;
							                                    using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbCommand))
                                                                {
                                                                    dataSet.Reset();
                                                                    dataAdapter.Fill(dataSet);
                                                                    dataTable = dataSet.Tables[0];
                                                                    if (dbCommand.Connection.State != ConnectionState.Closed)
												                    {
													                    dbCommand.Connection.Dispose();
												                    }
                                                                }
						                                    }
						                                    npsql.Close();
					                                    }
			 

										 
									        }catch{
										        throw;
									        }
									        return dataTable;
							        }
public virtual System.Data.DataTable lookup_RoomOccupancyStatus_ipdno(String tenantid,String patientname,string searchterm="", int? pagesize=1000, int? pagenumber=0)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_RoomOccupancyStatus_ipdno\"(@pvar_tenantid,@pvar_patientname,@pvar_searchterm,@pvar_pagesize,@pvar_pagenumber)", npsql))
						                                    {
                                            
                                                                dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);dbCommand.Parameters.AddWithValue("pvar_patientname",(object)patientname??DBNull.Value);  
																dbCommand.Parameters.AddWithValue("pvar_searchterm", (object)searchterm?? DBNull.Value);
                dbCommand.Parameters.AddWithValue("pvar_pagesize", (object)pagesize ?? DBNull.Value);
                dbCommand.Parameters.AddWithValue("pvar_pagenumber", (object)pagenumber ?? DBNull.Value);
																 
                                                                dbCommand.CommandType = CommandType.Text;
							                                    using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbCommand))
                                                                {
                                                                    dataSet.Reset();
                                                                    dataAdapter.Fill(dataSet);
                                                                    dataTable = dataSet.Tables[0];
                                                                    if (dbCommand.Connection.State != ConnectionState.Closed)
												                    {
													                    dbCommand.Connection.Dispose();
												                    }
                                                                }
						                                    }
						                                    npsql.Close();
					                                    }
			 

										 
									        }catch{
										        throw;
									        }
									        return dataTable;
							        }
public virtual System.Data.DataTable lookup_RoomOccupancyStatus_block(String tenantid)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_RoomOccupancyStatus_block\"(@pvar_tenantid)", npsql))
						                                    {
                                            
                                                                dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);  
																
																 
                                                                dbCommand.CommandType = CommandType.Text;
							                                    using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbCommand))
                                                                {
                                                                    dataSet.Reset();
                                                                    dataAdapter.Fill(dataSet);
                                                                    dataTable = dataSet.Tables[0];
                                                                    if (dbCommand.Connection.State != ConnectionState.Closed)
												                    {
													                    dbCommand.Connection.Dispose();
												                    }
                                                                }
						                                    }
						                                    npsql.Close();
					                                    }
			 

										 
									        }catch{
										        throw;
									        }
									        return dataTable;
							        }
public virtual System.Data.DataTable lookup_RoomOccupancyStatus_building(String tenantid,String block)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_RoomOccupancyStatus_building\"(@pvar_tenantid,@pvar_block)", npsql))
						                                    {
                                            
                                                                dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);dbCommand.Parameters.AddWithValue("pvar_block",(object)block??DBNull.Value);  
																
																 
                                                                dbCommand.CommandType = CommandType.Text;
							                                    using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbCommand))
                                                                {
                                                                    dataSet.Reset();
                                                                    dataAdapter.Fill(dataSet);
                                                                    dataTable = dataSet.Tables[0];
                                                                    if (dbCommand.Connection.State != ConnectionState.Closed)
												                    {
													                    dbCommand.Connection.Dispose();
												                    }
                                                                }
						                                    }
						                                    npsql.Close();
					                                    }
			 

										 
									        }catch{
										        throw;
									        }
									        return dataTable;
							        }
public virtual System.Data.DataTable lookup_RoomOccupancyStatus_floor(String tenantid,String block,String building)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_RoomOccupancyStatus_floor\"(@pvar_tenantid,@pvar_block,@pvar_building)", npsql))
						                                    {
                                            
                                                                dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);dbCommand.Parameters.AddWithValue("pvar_block",(object)block??DBNull.Value);dbCommand.Parameters.AddWithValue("pvar_building",(object)building??DBNull.Value);  
																
																 
                                                                dbCommand.CommandType = CommandType.Text;
							                                    using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbCommand))
                                                                {
                                                                    dataSet.Reset();
                                                                    dataAdapter.Fill(dataSet);
                                                                    dataTable = dataSet.Tables[0];
                                                                    if (dbCommand.Connection.State != ConnectionState.Closed)
												                    {
													                    dbCommand.Connection.Dispose();
												                    }
                                                                }
						                                    }
						                                    npsql.Close();
					                                    }
			 

										 
									        }catch{
										        throw;
									        }
									        return dataTable;
							        }
public virtual System.Data.DataTable lookup_RoomOccupancyStatus_room(String tenantid,String block,String building,String floor)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_RoomOccupancyStatus_room\"(@pvar_tenantid,@pvar_block,@pvar_building,@pvar_floor)", npsql))
						                                    {
                                            
                                                                dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);dbCommand.Parameters.AddWithValue("pvar_block",(object)block??DBNull.Value);dbCommand.Parameters.AddWithValue("pvar_building",(object)building??DBNull.Value);dbCommand.Parameters.AddWithValue("pvar_floor",(object)floor??DBNull.Value);  
																
																 
                                                                dbCommand.CommandType = CommandType.Text;
							                                    using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbCommand))
                                                                {
                                                                    dataSet.Reset();
                                                                    dataAdapter.Fill(dataSet);
                                                                    dataTable = dataSet.Tables[0];
                                                                    if (dbCommand.Connection.State != ConnectionState.Closed)
												                    {
													                    dbCommand.Connection.Dispose();
												                    }
                                                                }
						                                    }
						                                    npsql.Close();
					                                    }
			 

										 
									        }catch{
										        throw;
									        }
									        return dataTable;
							        }


						public virtual System.Data.DataTable lookup_change_RoomOccupancyStatus_patientvisit(string PatientVisitid)
						{
							DataTable dataTable=new DataTable();
                            DataSet dataSet=new DataSet();
							try
							{
								 	 
                                    using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
									{
										npsql.Open();
										using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_change_RoomOccupancyStatus_patientvisit\"(@pvar_patientvisitid)", npsql))
										{
											dbCommand.CommandType = CommandType.Text;
											dbCommand.Parameters.AddWithValue("pvar_patientvisitid",NpgsqlDbType.Varchar,(object)PatientVisitid??DBNull.Value);
											using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbCommand))
											{
												dataSet.Reset();
												dataAdapter.Fill(dataSet);
												dataTable = dataSet.Tables[0];
												if (dbCommand.Connection.State != ConnectionState.Closed)
												{
													dbCommand.Connection.Dispose();
												}
											}
										}
										npsql.Close();
									}

								 
							}catch{
								throw;
							}

							return dataTable;
						}






			    }


			    }
