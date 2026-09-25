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

			    //This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:10
			    public class RoomAllocationDAL
			    {
					public virtual string db_connectionstring{get;set;}
					
			 	    public RoomAllocationDAL(string connectionString)
				    {
						
					    db_connectionstring=connectionString;
				    }
				  
			        
				public virtual string Add_Room_Allocation(RoomAllocationModel model)
			  { 
				  String ResponseMessage="";
					 
					try{
							 
                            using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					        {
						        npsql.Open();
						        using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Add_Room_Allocation\"(@pvar_roomallocationid,@pvar_tenantid,@pvar_roomallocationno,@pvar_ipdno,@pvar_block,@pvar_building,@pvar_floor,@pvar_room,@pvar_fromdate,@pvar_todate,@pvar_status,@pvar_createduser)", npsql))
						        {
                                        dbCommand.CommandType = CommandType.Text;
						            	
								        					dbCommand.Parameters.AddWithValue("pvar_roomallocationid",NpgsqlDbType.Uuid,(object)model.RoomAllocationid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)model.tenantid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_roomallocationno",NpgsqlDbType.Varchar,(object)model.roomallocationno??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_ipdno",NpgsqlDbType.Uuid,(object)model.ipdno??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_block",NpgsqlDbType.Uuid,(object)model.block??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_building",NpgsqlDbType.Uuid,(object)model.building??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_floor",NpgsqlDbType.Uuid,(object)model.floor??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_room",NpgsqlDbType.Uuid,(object)model.room??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_fromdate",NpgsqlDbType.Date,(object)model.fromdate??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_todate",NpgsqlDbType.Date,(object)model.todate??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_status",NpgsqlDbType.Varchar,(object)model.status??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_createduser",NpgsqlDbType.Uuid,(object)model.createduser??DBNull.Value);	
															
										
                                        NpgsqlParameter outParm = new NpgsqlParameter("pvar_returnMessage", NpgsqlDbType.Varchar)
                                        {
                                             Direction = ParameterDirection.Output
                                        };
                                        dbCommand.Parameters.Add(outParm);

                                        dbCommand.ExecuteNonQuery();
								        ResponseMessage = outParm.Value.ToString();
										if (ResponseMessage == "201.1" && !string.IsNullOrWhiteSpace(model.bookedfor) && model.tenantid.HasValue && model.RoomAllocationid.HasValue)
										{
											using (var bookedForCommand = new NpgsqlCommand(
												"UPDATE RoomOccupancyStatus SET bookedfor = @pvar_bookedfor WHERE tenantid = @pvar_tenantid AND roomallocationno = @pvar_roomallocationid",
												npsql))
											{
												bookedForCommand.Parameters.AddWithValue("pvar_bookedfor", NpgsqlDbType.Varchar, model.bookedfor);
												bookedForCommand.Parameters.AddWithValue("pvar_tenantid", NpgsqlDbType.Uuid, model.tenantid.Value);
												bookedForCommand.Parameters.AddWithValue("pvar_roomallocationid", NpgsqlDbType.Varchar, model.RoomAllocationid.Value.ToString());
												bookedForCommand.ExecuteNonQuery();
											}
										}
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

		public virtual string SetManualRoomAvailability(ManualRoomAvailabilityModel model, Guid userId)
		{
			if (!model.tenantid.HasValue || model.tenantid.Value == Guid.Empty)
				return "Tenant is required.";
			if (model.room == Guid.Empty)
				return "Room is required.";
			if (model.fromdate.Date > model.todate.Date)
				return "To Date should be on or after From Date.";
			if (!string.Equals(model.status, "Blocked", StringComparison.OrdinalIgnoreCase) &&
				!string.Equals(model.status, "Available", StringComparison.OrdinalIgnoreCase))
				return "Availability status is invalid.";
			if (string.IsNullOrWhiteSpace(model.blockedfor))
				return "Blocked for is required.";

			using (var connection = new NpgsqlConnection(db_connectionstring))
			{
				connection.Open();
				using (var transaction = connection.BeginTransaction())
				{
					try
					{
						using (var authorization = new NpgsqlCommand("SELECT \"Check_Authorization\"(@user_id, 'RoomAllocation', 'create')", connection, transaction))
						{
							authorization.Parameters.AddWithValue("user_id", NpgsqlDbType.Uuid, userId);
							if (!Convert.ToBoolean(authorization.ExecuteScalar()))
								return "Authorization Failed";
						}

						Guid block;
						Guid building;
						Guid floor;
						using (var roomCommand = new NpgsqlCommand(@"
SELECT block, building, floor
FROM Room
WHERE Roomid = @room_id
  AND tenantid = @tenant_id
  AND COALESCE(isdeleted, false) = false
FOR SHARE", connection, transaction))
						{
							roomCommand.Parameters.AddWithValue("room_id", NpgsqlDbType.Uuid, model.room);
							roomCommand.Parameters.AddWithValue("tenant_id", NpgsqlDbType.Uuid, model.tenantid.Value);
							using (var reader = roomCommand.ExecuteReader())
							{
								if (!reader.Read() || reader.IsDBNull(0) || reader.IsDBNull(1) || reader.IsDBNull(2))
									return "The selected room is not fully linked to its room masters.";
								block = reader.GetGuid(0);
								building = reader.GetGuid(1);
								floor = reader.GetGuid(2);
							}
						}

						var reason = model.blockedfor.Trim();
						if (!string.IsNullOrWhiteSpace(model.description))
							reason += " | Description: " + model.description.Trim();

						if (string.Equals(model.status, "Available", StringComparison.OrdinalIgnoreCase))
						{
							if (!model.allocationid.HasValue)
								return "Select an existing manual block to make it available.";

							int releasedRows;
							using (var release = new NpgsqlCommand(@"
UPDATE RoomOccupancyStatus
SET status = 'Available', bookedfor = @reason, modifieduser = @user_id, modifieddate = NOW()
WHERE tenantid = @tenant_id
  AND room = @room_id
  AND roomallocationno = @allocation_id
  AND ipdno IS NULL
  AND status = 'Blocked'
  AND bookeddate BETWEEN @from_date AND @to_date", connection, transaction))
							{
								release.Parameters.AddWithValue("reason", NpgsqlDbType.Varchar, reason);
								release.Parameters.AddWithValue("user_id", NpgsqlDbType.Uuid, userId);
								release.Parameters.AddWithValue("tenant_id", NpgsqlDbType.Uuid, model.tenantid.Value);
								release.Parameters.AddWithValue("room_id", NpgsqlDbType.Uuid, model.room);
								release.Parameters.AddWithValue("allocation_id", NpgsqlDbType.Varchar, model.allocationid.Value.ToString());
								release.Parameters.AddWithValue("from_date", NpgsqlDbType.Date, model.fromdate.Date);
								release.Parameters.AddWithValue("to_date", NpgsqlDbType.Date, model.todate.Date);
								releasedRows = release.ExecuteNonQuery();
							}
							if (releasedRows == 0)
								return "No matching manual blocked dates were found.";

							using (var updateAllocation = new NpgsqlCommand(@"
UPDATE RoomAllocation ra
SET status = CASE WHEN EXISTS (
        SELECT 1 FROM RoomOccupancyStatus ros
        WHERE ros.tenantid = ra.tenantid
          AND ros.roomallocationno = ra.RoomAllocationid::varchar
          AND ros.status = 'Blocked'
    ) THEN 'Blocked' ELSE 'Available' END,
    modifieduser = @user_id,
    modifieddate = NOW()
WHERE ra.RoomAllocationid = @allocation_uuid
  AND ra.tenantid = @tenant_id
  AND ra.ipdno IS NULL", connection, transaction))
							{
								updateAllocation.Parameters.AddWithValue("user_id", NpgsqlDbType.Uuid, userId);
								updateAllocation.Parameters.AddWithValue("allocation_uuid", NpgsqlDbType.Uuid, model.allocationid.Value);
								updateAllocation.Parameters.AddWithValue("tenant_id", NpgsqlDbType.Uuid, model.tenantid.Value);
								updateAllocation.ExecuteNonQuery();
							}
						}
						else
						{
							using (var conflict = new NpgsqlCommand(@"
SELECT EXISTS (
    SELECT 1 FROM RoomOccupancyStatus
    WHERE tenantid = @tenant_id
      AND room = @room_id
      AND status IN ('Booked', 'Blocked', 'Occupied')
      AND bookeddate BETWEEN @from_date AND @to_date
)", connection, transaction))
							{
								conflict.Parameters.AddWithValue("tenant_id", NpgsqlDbType.Uuid, model.tenantid.Value);
								conflict.Parameters.AddWithValue("room_id", NpgsqlDbType.Uuid, model.room);
								conflict.Parameters.AddWithValue("from_date", NpgsqlDbType.Date, model.fromdate.Date);
								conflict.Parameters.AddWithValue("to_date", NpgsqlDbType.Date, model.todate.Date);
								if (Convert.ToBoolean(conflict.ExecuteScalar()))
									return "Room is already Booked, Blocked, or Occupied for the selected date range.";
							}

							var allocationId = Guid.NewGuid();
							string allocationNumber;
							using (var number = new NpgsqlCommand("SELECT generate_formatted_numbers(@tenant_id, 'YYYY-MM-9999', 'roomallocation', 'roomallocationno')", connection, transaction))
							{
								number.Parameters.AddWithValue("tenant_id", NpgsqlDbType.Uuid, model.tenantid.Value);
								allocationNumber = Convert.ToString(number.ExecuteScalar());
							}

							using (var insertAllocation = new NpgsqlCommand(@"
INSERT INTO RoomAllocation
(RoomAllocationid, tenantid, roomallocationno, ipdno, block, building, floor, room, fromdate, todate, status, createduser)
VALUES
(@allocation_id, @tenant_id, @allocation_number, NULL, @block, @building, @floor, @room_id, @from_date, @to_date, 'Blocked', @user_id)", connection, transaction))
							{
								insertAllocation.Parameters.AddWithValue("allocation_id", NpgsqlDbType.Uuid, allocationId);
								insertAllocation.Parameters.AddWithValue("tenant_id", NpgsqlDbType.Uuid, model.tenantid.Value);
								insertAllocation.Parameters.AddWithValue("allocation_number", NpgsqlDbType.Varchar, allocationNumber);
								insertAllocation.Parameters.AddWithValue("block", NpgsqlDbType.Uuid, block);
								insertAllocation.Parameters.AddWithValue("building", NpgsqlDbType.Uuid, building);
								insertAllocation.Parameters.AddWithValue("floor", NpgsqlDbType.Uuid, floor);
								insertAllocation.Parameters.AddWithValue("room_id", NpgsqlDbType.Uuid, model.room);
								insertAllocation.Parameters.AddWithValue("from_date", NpgsqlDbType.Date, model.fromdate.Date);
								insertAllocation.Parameters.AddWithValue("to_date", NpgsqlDbType.Date, model.todate.Date);
								insertAllocation.Parameters.AddWithValue("user_id", NpgsqlDbType.Uuid, userId);
								insertAllocation.ExecuteNonQuery();
							}

							using (var removeAvailable = new NpgsqlCommand(@"
DELETE FROM RoomOccupancyStatus
WHERE tenantid = @tenant_id AND room = @room_id AND ipdno IS NULL
  AND status = 'Available' AND bookeddate BETWEEN @from_date AND @to_date", connection, transaction))
							{
								removeAvailable.Parameters.AddWithValue("tenant_id", NpgsqlDbType.Uuid, model.tenantid.Value);
								removeAvailable.Parameters.AddWithValue("room_id", NpgsqlDbType.Uuid, model.room);
								removeAvailable.Parameters.AddWithValue("from_date", NpgsqlDbType.Date, model.fromdate.Date);
								removeAvailable.Parameters.AddWithValue("to_date", NpgsqlDbType.Date, model.todate.Date);
								removeAvailable.ExecuteNonQuery();
							}

							using (var insertOccupancy = new NpgsqlCommand(@"
INSERT INTO RoomOccupancyStatus
(RoomOccupancyStatusid, tenantid, roomallocationno, patientvisit, patientname, ipdno, block, building, floor, room, bookeddate, status, bookedfor, createduser, createddate)
SELECT gen_random_uuid(), @tenant_id, @allocation_reference, NULL, NULL, NULL,
       @block, @building, @floor, @room_id, day::date, 'Blocked', @reason, @user_id, NOW()
FROM generate_series(@from_date::date, @to_date::date, interval '1 day') day", connection, transaction))
							{
								insertOccupancy.Parameters.AddWithValue("tenant_id", NpgsqlDbType.Uuid, model.tenantid.Value);
								insertOccupancy.Parameters.AddWithValue("allocation_reference", NpgsqlDbType.Varchar, allocationId.ToString());
								insertOccupancy.Parameters.AddWithValue("block", NpgsqlDbType.Uuid, block);
								insertOccupancy.Parameters.AddWithValue("building", NpgsqlDbType.Uuid, building);
								insertOccupancy.Parameters.AddWithValue("floor", NpgsqlDbType.Uuid, floor);
								insertOccupancy.Parameters.AddWithValue("room_id", NpgsqlDbType.Uuid, model.room);
								insertOccupancy.Parameters.AddWithValue("from_date", NpgsqlDbType.Date, model.fromdate.Date);
								insertOccupancy.Parameters.AddWithValue("to_date", NpgsqlDbType.Date, model.todate.Date);
								insertOccupancy.Parameters.AddWithValue("reason", NpgsqlDbType.Varchar, reason);
								insertOccupancy.Parameters.AddWithValue("user_id", NpgsqlDbType.Uuid, userId);
								insertOccupancy.ExecuteNonQuery();
							}
						}

						transaction.Commit();
						return "201.1";
					}
					catch (Exception ex)
					{
						transaction.Rollback();
						Console.WriteLine(ex);
						return ex.Message;
					}
				}
			}
		}

		public virtual Guid? UpdateToDateAndGetId(Guid ipdno, Guid room, string bookedfor, DateTime newToDate, Guid modifieduser)
		{
			try
			{
				using (var npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var cmd = new NpgsqlCommand(
						"SELECT * FROM \"Update_Room_Allocation_ToDate\"(@pvar_ipdno,@pvar_room,@pvar_bookedfor,@pvar_todate,@pvar_modifieduser)", npsql))
					{
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.AddWithValue("pvar_ipdno", NpgsqlDbType.Uuid, ipdno);
						cmd.Parameters.AddWithValue("pvar_room", NpgsqlDbType.Uuid, room);
						cmd.Parameters.AddWithValue("pvar_bookedfor", NpgsqlDbType.Varchar, (object)bookedfor ?? DBNull.Value);
						cmd.Parameters.AddWithValue("pvar_todate", NpgsqlDbType.Date, newToDate.Date);
						cmd.Parameters.AddWithValue("pvar_modifieduser", NpgsqlDbType.Uuid, modifieduser);
						NpgsqlParameter outId = new NpgsqlParameter("pvar_roomallocationid", NpgsqlDbType.Uuid)
						{
							Direction = ParameterDirection.Output
						};
						NpgsqlParameter outMsg = new NpgsqlParameter("pvar_returnMessage", NpgsqlDbType.Varchar)
						{
							Direction = ParameterDirection.Output
						};
						cmd.Parameters.Add(outId);
						cmd.Parameters.Add(outMsg);
						cmd.ExecuteNonQuery();
						npsql.Close();
						if (outId.Value != null && outId.Value != DBNull.Value)
							return (Guid)outId.Value;
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
			return null;
		}

public virtual RoomAllocationModel getById_RoomAllocation(string RoomAllocationid)
									 {
										DataTable dataTable = new DataTable();
										DataSet dataSet = new DataSet();
										try{
												 
												using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
												{
													npsql.Open();
													using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_RoomAllocation\"(@pvar_roomallocationid)", npsql))
													{
														dbCommand.CommandType = CommandType.Text;
														dbCommand.Parameters.AddWithValue("pvar_roomallocationid",(object)RoomAllocationid??DBNull.Value);
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
											return ModelConverter.ConvertDataRowToModel<RoomAllocationModel>(row);
										}
										else
										{
											return null;
										}
									 }
			 public virtual string  Update_Room_Allocation(RoomAllocationModel model)
			 { 
				 String ResponseMessage="";
					try{
						 	 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Update_Room_Allocation\"(@pvar_roomallocationid,@pvar_tenantid,@pvar_roomallocationno,@pvar_ipdno,@pvar_block,@pvar_building,@pvar_floor,@pvar_room,@pvar_fromdate,@pvar_todate,@pvar_status,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
															dbCommand.Parameters.AddWithValue("pvar_roomallocationid",NpgsqlDbType.Uuid,(object)model.RoomAllocationid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)model.tenantid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_roomallocationno",NpgsqlDbType.Varchar,(object)model.roomallocationno??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_ipdno",NpgsqlDbType.Uuid,(object)model.ipdno??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_block",NpgsqlDbType.Uuid,(object)model.block??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_building",NpgsqlDbType.Uuid,(object)model.building??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_floor",NpgsqlDbType.Uuid,(object)model.floor??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_room",NpgsqlDbType.Uuid,(object)model.room??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_fromdate",NpgsqlDbType.Date,(object)model.fromdate??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_todate",NpgsqlDbType.Date,(object)model.todate??DBNull.Value);

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
public virtual string  Remove_Room_Allocation(string id,string loginUserID)
			  { 
				  String ResponseMessage="";
					try{ 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Remove_Room_Allocation\"(@pvar_roomallocationid,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
										dbCommand.Parameters.AddWithValue("pvar_roomallocationid",(object)id??DBNull.Value);
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
public virtual JObject Room_Allocation_List(string tenantid
,string ipdno="",string block="",string building="",string floor="",string room="", int? pagesize=1000 , int? pagenumber=0,string searchterm="",string  sort_fields="")
			  { 
				  object dalResponse = null;
			
					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM\"Room_Allocation_List\"(@pvar_tenantid,@pvar_ipdno,@pvar_block,@pvar_building,@pvar_floor,@pvar_room,@pvar_pagesize,@pvar_pagenumber,@pvar_searchterm,@pvar_sort_fields)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
																dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_ipdno",(object)ipdno??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_block",(object)block??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_building",(object)building??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_floor",(object)floor??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_room",(object)room??DBNull.Value);

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
			   
			 
public virtual System.Data.DataTable get_all_RoomAllocation(string tenantid,string searchterm="", int? pagesize=1000, int? pagenumber=0)
			  { 

				    DataTable dataTable = new DataTable();
					DataSet dataSet = new DataSet();

					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"get_all_RoomAllocation\"(@pvar_tenantid)", npsql))
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
public virtual System.Data.DataTable getById_allinfo_RoomAllocation(string RoomAllocationid)
			 {
				DataSet dataSet=new DataSet();
				DataTable dataTable = new DataTable();
				try{
					     
						using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
						{
							npsql.Open();
							using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_all_RoomAllocation\"(@pvar_roomallocationid)", npsql))
							{
								dbCommand.CommandType = CommandType.Text;
								dbCommand.Parameters.AddWithValue("pvar_roomallocationid",(object)RoomAllocationid??DBNull.Value);
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
			  
public virtual System.Data.DataTable lookup_RoomAllocation_ipdno(String tenantid,string searchterm="", int? pagesize=1000, int? pagenumber=0)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_RoomAllocation_ipdno\"(@pvar_tenantid,@pvar_searchterm,@pvar_pagesize,@pvar_pagenumber)", npsql))
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
public virtual System.Data.DataTable lookup_RoomAllocation_block(String tenantid)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_RoomAllocation_block\"(@pvar_tenantid)", npsql))
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
public virtual System.Data.DataTable lookup_RoomAllocation_building(String tenantid,String block)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_RoomAllocation_building\"(@pvar_tenantid,@pvar_block)", npsql))
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
public virtual System.Data.DataTable lookup_RoomAllocation_floor(String tenantid,String block,String building)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_RoomAllocation_floor\"(@pvar_tenantid,@pvar_block,@pvar_building)", npsql))
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
public virtual System.Data.DataTable lookup_RoomAllocation_room(String tenantid,String block,String building,String floor)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_RoomAllocation_room\"(@pvar_tenantid,@pvar_block,@pvar_building,@pvar_floor)", npsql))
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







			    }


			    }
