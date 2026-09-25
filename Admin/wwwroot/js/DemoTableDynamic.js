(function(namespace, $) {
  "use strict";

  var DemoTableDynamic = function() {
    // Create reference to this instance
    var o = this;
    // Initialize app when document is ready
    $(document).ready(function() {
      o.initialize();
    });

  };
  var p = DemoTableDynamic.prototype;

  // =========================================================================
  // INIT
  // =========================================================================

  p.initialize = function() {
    this._initDataTables();
  };

  // =========================================================================
  // DATATABLES
  // =========================================================================

  p._initDataTables = function() {
    if (!$.isFunction($.fn.dataTable)) {
      return;
    }

    // Init the demo DataTables
    this._createDataTable1();
    this._createDataTable2();
  };

  p._createDataTable1 = function() {
    $('#datatable1').DataTable({
      "dom": 'lCfrtip',
      "order": [],
      "colVis": {
        "buttonText": "Columns",
        "overlayFade": 0,
        "align": "right"
      },
      "language": {
        "lengthMenu": '_MENU_ entries per page',
        "search": '<i class="fa fa-search"></i>',
        "paginate": {
          "previous": '<i class="fa fa-angle-left"></i>',
          "next": '<i class="fa fa-angle-right"></i>'
        }
      }
    });

    $('#datatable1 tbody').on('click', 'tr', function() {
      $(this).toggleClass('selected');
    });
  };

  p._createDataTable2 = function() {
    var buttonCommon = {
      exportOptions: {
        format: {
          body: function(data, row, column, node) {
            // Strip $ from salary column to make it numeric
            return column === 5 ?
              data.replace(/[$,]/g, '') :
              data;
          }
        }
      }
    };

    var table = $('#datatable2').DataTable({
      dom: 'Bfrtip',
      "ajax": $('#datatable2').data('source'),
      "colVis": {
        "buttonText": `<span class="material-icons">
        settings
        </span>`,
        "overlayFade": 0,
        "align": "right"
      },
      "columns": [
        { "data": "name" },
        { "data": "position" },
        { "data": "office" },
        { "data": "salary" }
      ],
      "tableTools": {
        "sSwfPath": $('#datatable2').data('swftools')
      },
      "order": [
        [1, 'asc']
      ],
      autoWidth: !1,
      responsive: !0,
      "language": {
        "lengthMenu": '_MENU_ entries per page',
        search: '<i class="fa fa-search"></i>',
        searchPlaceholder: "Search for records...",
        "paginate": {
          "previous": '<i class="fa fa-angle-left"></i>',
          "next": '<i class="fa fa-angle-right"></i>'
        }
      },
      initComplete: function(a, b) {
        $('tbody', '#datatable2').children('tr').first().addClass('active-row');
      }
    });

    //Add event listener for opening and closing details
    var o = this;
    $('#datatable2 tbody').on('click', 'td.details-control', function() {
      var tr = $(this).closest('tr');
      var row = table.row(tr);

      if (row.child.isShown()) {
        // This row is already open - close it
        row.child.hide();
        tr.removeClass('shown');
      } else {
        // Open this row
        row.child(o._formatDetails(row.data())).show();
        tr.addClass('shown');
      }
    });
  };

  var dataTable = $("#data-table").DataTable({
    dom: 'Bfrtip',
    columnDefs: [{
      targets: [0],
      orderData: [0, 1]
    }, {
      targets: [1],
      orderData: [1, 0]
    }, {
      targets: [4],
      orderData: [4, 0]
    }],
    autoWidth: !1,
    responsive: !0,
    lengthMenu: [
      [15, 30, 45, -1],
      ["15 Rows", "30 Rows", "45 Rows", "Everything"]
    ],
    language: { search: '<i class="fa fa-search"></i>', searchPlaceholder: "Search for records..." },
    sDom: '<"dataTables__top"lfB>rt<"dataTables__bottom"ip><"clear">',
    buttons: [{ extend: "excelHtml5", title: "Export Data" }, { extend: "csvHtml5", title: "Export Data" }, { extend: "print", title: "Material Admin" }],
    // buttons: [{ extend: "excelHtml5", title: "Export Data" }, { extend: "csvHtml5", title: "Export Data" }, { extend: "print", title: "Material Admin" }],
    initComplete: function(a, b) {
      $(this).closest(".dataTables_wrapper").find(".dt-buttons").hide();
      $(this).closest(".dataTables_wrapper").find(".dataTables__top").prepend(`
        <div class="dataTables_buttons hidden-sm-down actions">
          <div class="input-group prefix">
            <span class="input-group-addon">
              <i class="material-icons">search</i>
            </span>
            <input type="text" class="form-control dt-global-search" placeholder="Search..." />
          </div>
          
          <div class="dropdown actions__item">
            <i data-toggle="dropdown" class="material-icons">share</i>
            <ul class="dropdown-menu dropdown-menu-right">
              <a href="javascript: void 0" class="dropdown-item" data-table-action="excel">Excel (.xlsx)</a>
              <a href="javascript: void 0" class="dropdown-item" data-table-action="csv">CSV (.csv)</a>
              <a href="javascript: void 0" class="dropdown-item" data-table-action="print">Print</a>
            </ul>
          </div>
          <div class="dropdown actions__item">
            <i data-toggle="dropdown" class="material-icons">settings</i>
            <ul class="dropdown-menu dropdown-menu-right ColVis_collection">
              <li>
                <label>
                  <span class="material-icons">drag_indicator</span>
                  Name <input type="checkbox" class="colvis_switch" data-column="0" checked />
                </label>
              </li>
              <li>
                <label>
                  <span class="material-icons">drag_indicator</span>
                  Position <input type="checkbox" class="colvis_switch" data-column="1" checked />
                </label>
              </li>
              <li>
                <label>
                  <span class="material-icons">drag_indicator</span>
                  Office <input type="checkbox" class="colvis_switch" data-column="2" checked />
                </label>
              </li>
              <li>
                <label>
                  <span class="material-icons">drag_indicator</span>
                  Age <input type="checkbox" class="colvis_switch" data-column="3" checked />
                </label>
              </li>
            </ul>
          </div>
          <button class="btn btn-rounded add-row-btn" type="button">
            <span class="add-btn-text">Add Row</span> <span class="material-icons">add</span>
          </button>
        </div>
      `);
      $(this).closest(".dataTables_wrapper").find(".dataTables_length").prepend(`
        <i class="actions__item mdi material-icons">filter_alt</i>
      `);
      if ($(this).hasClass('make-active')) {
        $('tbody', $(this)).children('tr').first().addClass('active-row');
        setTimeout(() => {
          $('.active-row td:visible:last').addClass('last-active-td');
        }, 200);
      }
      $(this).closest(".dataTables_wrapper").find(".dt-global-search").on('keyup', function() {
        dataTable.search(this.value).draw();
      });
    }
  });

  var table = $('#alphabet_table').DataTable({
    dom: ['Alfrtip', 'Bfrtip'],
    alphabetSearch: {
      column: 0
    },
    columnDefs: [{
      targets: [0],
      orderData: [0, 1]
    }, {
      targets: [1],
      orderData: [1, 0]
    }, {
      targets: [4],
      orderData: [4, 0]
    }],
    autoWidth: !1,
    responsive: !0,
    lengthMenu: [
      [15, 30, 45, -1],
      ["15 Rows", "30 Rows", "45 Rows", "Everything"]
    ],
    language: { search: '<i class="fa fa-search"></i>', searchPlaceholder: "Search for records..." },
    //Bfrtip Alfrtip
    sDom: '<"dataTables__top"lfAB>rt<"dataTables__bottom"ip><"clear">',
    // sDom: '<"dataTables__top"lfB>rt<"dataTables__bottom"ip><"clear">',
    buttons: [{ extend: "excelHtml5", title: "Export Data" }, { extend: "csvHtml5", title: "Export Data" }, { extend: "print", title: "Material Admin" }],
    initComplete: function(a, b) {
      $(this).closest(".dataTables_wrapper").find(".dt-buttons").hide();
      $(this).closest(".dataTables_wrapper").find(".dataTables__top").prepend(`
        <div class="dataTables_buttons hidden-sm-down actions">
          <div class="input-group prefix">
            <span class="input-group-addon">
              <i class="material-icons">search</i>
            </span>
            <input type="text" class="form-control dt-global-search" placeholder="Search..." />
          </div>
          
          <div class="dropdown actions__item">
            <i data-toggle="dropdown" class="material-icons">share</i>
            <ul class="dropdown-menu dropdown-menu-right">
              <a href="javascript: void 0" class="dropdown-item" data-table-action="excel">Excel (.xlsx)</a>
              <a href="javascript: void 0" class="dropdown-item" data-table-action="csv">CSV (.csv)</a>
              <a href="javascript: void 0" class="dropdown-item" data-table-action="print">Print</a>
            </ul>
          </div>
          <div class="dropdown actions__item">
            <i data-toggle="dropdown" class="material-icons">settings</i>
            <ul class="dropdown-menu dropdown-menu-right ColVis_collection">
              <li>
                <label>
                  <span class="material-icons">drag_indicator</span>
                  Name <input type="checkbox" class="colvis_switch" data-column="0" checked />
                </label>
              </li>
              <li>
                <label>
                  <span class="material-icons">drag_indicator</span>
                  Position <input type="checkbox" class="colvis_switch" data-column="1" checked />
                </label>
              </li>
              <li>
                <label>
                  <span class="material-icons">drag_indicator</span>
                  Office <input type="checkbox" class="colvis_switch" data-column="2" checked />
                </label>
              </li>
              <li>
                <label>
                  <span class="material-icons">drag_indicator</span>
                  Age <input type="checkbox" class="colvis_switch" data-column="3" checked />
                </label>
              </li>
            </ul>
          </div>
          <button class="btn btn-rounded add-row-btn" type="button">
            <span class="add-btn-text">Add Row</span> <span class="material-icons">add</span>
          </button>
        </div>
      `);
      $(this).closest(".dataTables_wrapper").find(".dataTables_length").prepend(`
        <i class="actions__item mdi material-icons">filter_alt</i>
      `);
      if ($(this).hasClass('make-active')) {
        $('tbody', $(this)).children('tr').first().addClass('active-row');
        setTimeout(() => {
          $('.active-row td:visible:last').addClass('last-active-td');
        }, 200);
      }
      $(this).closest(".dataTables_wrapper").find(".dt-global-search").on('keyup', function() {
        dataTable.search(this.value).draw();
      });
    }
  });

  $('.ColVis_collection li').on('click', function(event) {
    event.stopPropagation();
  });

  $('.colvis_switch').on('change', function(e) {
    var column = dataTable.column($(this).attr('data-column'));
    column.visible(!column.visible());
  });

  $(".dataTables_filter input[type=search]").focus(function() {
    $(this).closest(".dataTables_filter").addClass("dataTables_filter--toggled")
  }), $(".dataTables_filter input[type=search]").blur(function() {
    $(this).closest(".dataTables_filter").removeClass("dataTables_filter--toggled")
  })

  $("body").on("click", "[data-table-action]", function(a) {
    a.preventDefault();
    var b = $(this).data("table-action");
    console.log('dataTables_wrapper', $(this).closest(".dataTables_wrapper").find(".buttons-print"));
    if ("excel" === b && $(this).closest(".dataTables_wrapper").find(".buttons-excel").trigger("click"), "csv" === b && $(this).closest(".dataTables_wrapper").find(".buttons-csv").trigger("click"), "print" === b && $(this).closest(".dataTables_wrapper").find(".buttons-print").trigger("click"), "fullscreen" === b) {
      var c = $(this).closest(".card");
      c.hasClass("card--fullscreen") ? (c.removeClass("card--fullscreen"), $("body").removeClass("data-table-toggled")) : (c.addClass("card--fullscreen"), $("body").addClass("data-table-toggled"))
    }
  });

  // =========================================================================
  // DETAILS
  // =========================================================================

  p._formatDetails = function(d) {
    // `d` is the original data object for the row
    return '<table cellpadding="5" cellspacing="0" border="0" style="padding-left:50px;">' +
      '<tr>' +
      '<td>Full name:</td>' +
      '<td>' + d.name + '</td>' +
      '</tr>' +
      '<tr>' +
      '<td>Extension number:</td>' +
      '<td>' + d.extn + '</td>' +
      '</tr>' +
      '<tr>' +
      '<td>Extra info:</td>' +
      '<td>And any further details here (images etc)...</td>' +
      '</tr>' +
      '</table>';
  };

  // =========================================================================
  namespace.DemoTableDynamic = new DemoTableDynamic;
}(this.materialadmin, jQuery)); // pass in (namespace, jQuery):