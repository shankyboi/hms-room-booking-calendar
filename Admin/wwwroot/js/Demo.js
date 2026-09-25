(function(namespace, $) {
  "use strict";

  var Demo = function() {
    // Create reference to this instance
    var o = this;
    // Initialize app when document is ready
    $(document).ready(function() {
      o.initialize();
    });

  };
  var p = Demo.prototype;

  // =========================================================================
  // INIT
  // =========================================================================

  p.initialize = function() {
    this._enableEvents();

    this._initButtonStates();
    this._initIconSearch();
    this._initInversedTogglers();
    this._initChatMessage();
    this._initMenuDropDown();
    this._initProjectCardHover();
  };



  // =========================================================================
  // EVENTS
  // =========================================================================

  // events
  p._enableEvents = function() {
    var o = this;

    $('.card-head .tools .btn-refresh').on('click', function(e) {
      o._handleCardRefresh(e);
    });
    $('.card-head .tools .btn-collapse').on('click', function(e) {
      o._handleCardCollapse(e);
    });
    $('.card-head .tools .btn-close').on('click', function(e) {
      o._handleCardClose(e);
    });
    $('.card-head .tools .menu-card-styling a').on('click', function(e) {
      o._handleCardStyling(e);
    });
    $('.theme-selector a').on('click', function(e) {
      o._handleThemeSwitch(e);
    });

    $('.dropdown-menu li a.has-submenu').on('click', function(event) {
      event.stopPropagation();
      $(this).closest('.dropdown').addClass('open');
    });

    $('[data-toggle="submenu"]').on('click', function(event) {
      let loc_sef = $(this),
        loc_submenu = loc_sef.next('.main-submenu');
      loc_submenu.slideToggle();
    })
  };

  // =========================================================================
  // CARD ACTIONS
  // =========================================================================

  p._handleCardRefresh = function(e) {
    var o = this;
    var card = $(e.currentTarget).closest('.card');
    materialadmin.AppCard.addCardLoader(card);
    setTimeout(function() {
      materialadmin.AppCard.removeCardLoader(card);
    }, 1500);
  };

  p._handleCardCollapse = function(e) {
    var card = $(e.currentTarget).closest('.card');
    materialadmin.AppCard.toggleCardCollapse(card);
  };

  p._handleCardClose = function(e) {
    var card = $(e.currentTarget).closest('.card');
    materialadmin.AppCard.removeCard(card);
  };

  p._handleCardStyling = function(e) {
    // Get selected style and active card
    var newStyle = $(e.currentTarget).data('style');
    var card = $(e.currentTarget).closest('.card');

    // Display the selected style in the dropdown menu
    $(e.currentTarget).closest('ul').find('li').removeClass('active');
    $(e.currentTarget).closest('li').addClass('active');

    // Find all cards with a 'style-' class
    var styledCard = card.closest('[class*="style-"]');

    if (styledCard.length > 0 && (!styledCard.hasClass('style-white') && !styledCard.hasClass('style-transparent'))) {
      // If a styled card is found, replace the style with the selected style
      // Exclude style-white and style-transparent
      styledCard.attr('class', function(i, c) {
        return c.replace(/\bstyle-\S+/g, newStyle);
      });
    } else {
      // Create variable to check if a style is switched
      var styleSwitched = false;

      // When no cards are found with a style, look inside the card for styled headers or body
      card.find('[class*="style-"]').each(function() {
        // Replace the style with the selected style
        // Exclude style-white and style-transparent
        if (!$(this).hasClass('style-white') && !$(this).hasClass('style-transparent')) {
          $(this).attr('class', function(i, c) {
            return c.replace(/\bstyle-\S+/g, newStyle);
          });
          styleSwitched = true;
        }
      });

      // If no style is switched, add 1 to the main Card
      if (styleSwitched === false) {
        card.addClass(newStyle);
      }
    }
  };

  // =========================================================================
  // COLOR SWITCHER
  // =========================================================================

  p._handleThemeSwitch = function(e) {
    e.preventDefault();
    var newTheme = $(e.currentTarget).attr('href');
    this.switchTheme(newTheme);
  };

  p.switchTheme = function(theme) {
    $('link').each(function() {
      var href = $(this).attr('href');
      href = href.replace(/(assets\/css\/)(.*)(\/)/g, 'assets/css/' + theme + '/');
      $(this).attr('href', href);
    });
  };

  // =========================================================================
  // CHAT MESSAGE
  // =========================================================================

  p._initChatMessage = function(e) {
    var o = this;
    $('#sidebarChatMessage').keydown(function(e) {
      o._handleChatMessage(e);
    });
  };

  p._handleChatMessage = function(e) {
    var input = $(e.currentTarget);

    // Detect enter
    if (e.keyCode === 13) {
      e.preventDefault();

      // Get chat message
      var demoTime = new Date().getHours() + ':' + new Date().getMinutes();
      var demoImage = $('.list-chats li img').attr('src');

      // Create html
      var html = '';
      html += '<li>';
      html += '	<div class="chat">';
      html += '		<div class="chat-avatar"><img class="img-circle" src="' + demoImage + '" alt=""></div>';
      html += '		<div class="chat-body">';
      html += '			' + input.val();
      html += '			<small>' + demoTime + '</small>';
      html += '		</div>';
      html += '	</div>';
      html += '</li>';
      var $new = $(html).hide();

      // Add to chat list
      $('.list-chats').prepend($new);

      // Animate new inserts
      $new.show('fast');

      // Reset chat input
      input.val('').trigger('autosize.resize');

      // Refresh for correct scroller size
      $('.offcanvas').trigger('refresh');
    }
  };

  // =========================================================================
  // INVERSE UI TOGGLERS
  // =========================================================================

  p._initInversedTogglers = function() {
    var o = this;


    $('input[name="menubarInversed"]').on('change', function(e) {
      o._handleMenubarInversed(e);
    });
    $('input[name="headerInversed"]').on('change', function(e) {
      o._handleHeaderInversed(e);
    });
  };

  p._handleMenubarInversed = function(e) {
    if ($(e.currentTarget).val() === '1') {
      $('#menubar').addClass('menubar-inverse');
    } else {
      $('#menubar').removeClass('menubar-inverse');
    }
  };
  p._handleHeaderInversed = function(e) {
    if ($(e.currentTarget).val() === '1') {
      $('#header').addClass('header-inverse');
    } else {
      $('#header').removeClass('header-inverse');
    }
  };

  // =========================================================================
  // BUTTON STATES (LOADING)
  // =========================================================================

  p._initButtonStates = function() {
    $('.btn-loading-state').click(function() {
      var btn = $(this);
      btn.button('loading');
      setTimeout(function() {
        btn.button('reset');
      }, 3000);
    });
  };

  p._initMenuDropDown = function() {
    let clickedMenu;
    $('.has-dropdown > a', '#main-menu').click(function(event) {
      event.preventDefault();
      var parentLi = $(this).parent();
      var wasOpen = parentLi.hasClass('open');
      $('.has-dropdown.open', '#main-menu').removeClass('open');
      if (wasOpen) {
        $('#menubar').removeClass('sidemenu-opened');
      } else {
        $(this).closest('#menubar').addClass('sidemenu-opened');
        parentLi.addClass('open');
      }
      // clickedMenu = $(this);
      // var loc_self = $(this),
      //   loc_dropdown_menu = $(loc_self).next('.menu-dropdown'),
      //   loc_dropdown = loc_dropdown_menu.clone(),
      //   loc_pos = loc_self.parent().position();
      // console.log('loc_pos', loc_self.parent().scrollTop());
      // $('#dropdown_container').remove();
      // $('#menubar').append('<div id="dropdown_container"></div>')
      // $('#dropdown_container').append(loc_dropdown);
      // $('#dropdown_container').css({
      //   position: 'absolute',
      //   top: loc_pos.top - 30,
      //   left: loc_pos.left + 90
      // });
    });

    $('[data-toggle="submenu"]').click(function(e) {
      e.preventDefault();
      const loc_self = $(this);
      loc_self.toggleClass('submenu-open');
      loc_self.next('.main-menu-submenu').slideToggle();
    });

    $('body').click(function(e) {
      if (!$(e.target).closest('.has-dropdown').length) {
        $('.has-dropdown.open', '#main-menu').removeClass('open');
        $('#menubar').removeClass('sidemenu-opened');
      }
      // if (!$(e.target).closest('.menubar-toggle').length && !$(e.target).closest('.menu-dropdown').length) {
      //   $('body').removeClass('menubar-open');
      // }
      // console.log('e.target', e.target);
      // if ($(e.target).attr('data-toggle') === 'submenu') {
      //   const loc_self = $(e.target);
      //   loc_self.toggleClass('submenu-open');
      //   loc_self.next('.main-submenu').slideToggle();
      // }

    });
  };

  // =========================================================================
  // ICON SEARCH
  // =========================================================================

  p._initIconSearch = function() {
    if ($('#iconsearch').length === 0) {
      return;
    }

    $('#iconsearch').focus();
    $('#iconsearch').on('keyup', function() {
      var val = $('#iconsearch').val();
      $('.col-md-3').hide();
      $('.col-md-3:contains("' + val + '")').each(function(e) {
        $(this).show();
      });

      $('.card').hide();
      $('.card:contains("' + val + '")').each(function(e) {
        $(this).show();
      });
    });
  };

  p._initProjectCardHover = function() {
    if ($('.project-card-hover').length === 0) {
      return;
    }
    $('.project-hover-section').append('<div class="project-details-hover-container"></div>');
    $('.project-card-hover').click(function() {
      let parent = $(this).closest('.project-hover-section');
      const parentHeight = parent.height();
      const hoverContainer = $('.project-details-hover-container', parent);
      hoverContainer.html($(this).clone().addClass('hovered')).show();
      $('.tools > .btn-group', hoverContainer).prepend(`
        <a href="#" class="btn btn-icon-close">
          <i class="material-icons">close</i>
        </a>
      `);
      hoverContainer.css({
        height: parentHeight - 15,
        width: $(this).width() + 20,
        position: 'absolute',
        left: $(this).offset().left - parent.offset().left,
        top: $(this).offset().top - parent.offset().top
      });
      $('.btn-icon-close', '.project-details-hover-container').on('click', function(event) {
        event.preventDefault();
        $('.project-details-hover-container', parent).empty().removeAttr('style').hide();
      });
    });

    $('body').click(function(event) {
      if (!$(event.target).closest('.project-hover-section').length) {
        let parent = $(event.target).closest('.project-hover-section');
        $('.project-details-hover-container', parent).empty().removeAttr('style').hide();
      }
    });

  }

  // =========================================================================
  namespace.Demo = new Demo;
}(this.materialadmin, jQuery)); // pass in (namespace, jQuery):