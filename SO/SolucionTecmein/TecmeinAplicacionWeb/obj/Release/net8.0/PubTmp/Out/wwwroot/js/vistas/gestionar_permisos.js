function renderMenuTree(menus, container) {
    let html = '<table class="table table-bordered table-hover table-rounded">';
    html += '<thead class="thead-dark"><tr>';
    html += '<th style="width: 50%;"><input type="checkbox" id="checkAllMenus"> Menú</th>';
    html += '<th style="width: 12.5%; text-align: center;"><input type="checkbox" id="checkAllCrear"> Crear</th>';
    html += '<th style="width: 12.5%; text-align: center;"><input type="checkbox" id="checkAllLeer"> Leer</th>';
    html += '<th style="width: 12.5%; text-align: center;"><input type="checkbox" id="checkAllActualizar"> Actualizar</th>';
    html += '<th style="width: 12.5%; text-align: center;"><input type="checkbox" id="checkAllEliminar"> Eliminar</th>';
    html += '</tr></thead>';
    html += '<tbody>';

    menus.forEach(menu => {
        html += renderMenuItem(menu, 0);
    });

    html += '</tbody>';
    html += '</table>';
    container.html(html);

    // Eventos para expandir/colapsar
    container.on('click', '.toggle-submenu', function () {
        const $row = $(this).closest('tr');
        const secMenu = $row.data('sec-menu');
        $('tr[data-parent-menu="' + secMenu + '"]').toggle(200);
        $(this).find('i').toggleClass('fa-chevron-right fa-chevron-down');
    });

    // Evento para marcar/desmarcar todos los menús
    $('#checkAllMenus').on('change', function () {
        const isChecked = $(this).prop('checked');
        $('.menu-checkbox').prop('checked', isChecked).trigger('change'); // Trigger change para propagar
    });

    // Eventos para marcar/desmarcar todos los permisos CRUD por columna
    $('#checkAllCrear').on('change', function () {
        const isChecked = $(this).prop('checked');
        $('input.permiso-checkbox[value="Crear"]').prop('checked', isChecked).trigger('change');
    });
    $('#checkAllLeer').on('change', function () {
        const isChecked = $(this).prop('checked');
        $('input.permiso-checkbox[value="Leer"]').prop('checked', isChecked).trigger('change');
    });
    $('#checkAllActualizar').on('change', function () {
        const isChecked = $(this).prop('checked');
        $('input.permiso-checkbox[value="Actualizar"]').prop('checked', isChecked).trigger('change');
    });
    $('#checkAllEliminar').on('change', function () {
        const isChecked = $(this).prop('checked');
        $('input.permiso-checkbox[value="Eliminar"]').prop('checked', isChecked).trigger('change');
    });

    // Eventos para checkboxes de fila
    container.on('change', '.menu-checkbox', function () {
        const isChecked = $(this).prop('checked');
        const $row = $(this).closest('tr');
        const secMenu = $row.data('sec-menu');

        // Marcar/desmarcar todos los permisos CRUD de esta fila
        $row.find('.permiso-checkbox').prop('checked', isChecked);

        // Marcar/desmarcar submenús y sus permisos
        $('tr[data-parent-menu="' + secMenu + '"]').each(function () {
            $(this).find('.menu-checkbox, .permiso-checkbox').prop('checked', isChecked);
        });

        // Propagar hacia arriba: si un hijo se marca, los padres también
        if (isChecked) {
            let currentParentSecMenu = $row.data('parent-menu');
            while (currentParentSecMenu) {
                const $parentRow = $('tr[data-sec-menu="' + currentParentSecMenu + '"]');
                $parentRow.find('.menu-checkbox').prop('checked', true);
                currentParentSecMenu = $parentRow.data('parent-menu');
            }
        }
    });

    container.on('change', '.permiso-checkbox', function () {
        const isChecked = $(this).prop('checked');
        const $row = $(this).closest('tr');
        const secMenu = $row.data('sec-menu');
        const $menuCheckbox = $row.find('.menu-checkbox');

        if (isChecked) {
            // Si un permiso se marca, el menú padre debe marcarse
            $menuCheckbox.prop('checked', true);
            // Propagar hacia arriba
            let currentParentSecMenu = $row.data('parent-menu');
            while (currentParentSecMenu) {
                const $parentRow = $('tr[data-sec-menu="' + currentParentSecMenu + '"]');
                $parentRow.find('.menu-checkbox').prop('checked', true);
                currentParentSecMenu = $parentRow.data('parent-menu');
            }
        } else {
            // Si un permiso se desmarca, verificar si quedan otros permisos o submenús marcados en el mismo nivel
            const hasOtherCheckedPermisos = $row.find('.permiso-checkbox:checked').length > 0;
            const hasCheckedSubmenus = $('tr[data-parent-menu="' + secMenu + '"]').find('.menu-checkbox:checked').length > 0;

            if (!hasOtherCheckedPermisos && !hasCheckedSubmenus) {
                $menuCheckbox.prop('checked', false);
            }
        }
    });
}

function renderMenuItem(menu, level) {
    const hasSubmenus = menu.subMenus && menu.subMenus.$values && menu.subMenus.$values.length > 0;
    const baseIndent = 20; // Indentación base por nivel
    const currentIndent = level * baseIndent; // Indentación total para el nivel actual

    let html = `<tr data-sec-menu="${menu.secMenu}" ${menu.secMenuPadre ? 'data-parent-menu="' + menu.secMenuPadre + '"' : ''} style="${menu.secMenuPadre ? 'display: none;' : ''}">`;

    // Celda del Menú
    html += '<td style="width: 50%;">';
    html += '<div class="d-flex align-items-center" style="padding-left: ' + currentIndent + 'px;">';
    if (hasSubmenus) {
        html += '<span class="toggle-submenu mr-2" style="cursor: pointer;"><i class="fas fa-chevron-right"></i></span>';
    } else {
        html += '<span class="mr-2" style="width: 16px;"></span>'; // Espacio para alinear
    }
    html += `<input type="checkbox" class="menu-checkbox mr-2" value="${menu.secMenu}" ${menu.verMenu ? 'checked' : ''} data-sec-menu="${menu.secMenu}">`;
    html += `<i class="${menu.icono} mr-2"></i>`;
    html += `<span class="font-weight-bold">${menu.descripcion}</span>`;
    html += '</div>';
    html += '</td>';

    // Celdas de Permisos CRUD (solo si tiene un padre)
    const crudPermisosHtml = menu.secMenuPadre !== null ?
        `
        <td style="width: 12.5%; text-align: center;"><input type="checkbox" class="permiso-checkbox" value="Crear" ${menu.crear ? 'checked' : ''} data-sec-menu="${menu.secMenu}"></td>
        <td style="width: 12.5%; text-align: center;"><input type="checkbox" class="permiso-checkbox" value="Leer" ${menu.leer ? 'checked' : ''} data-sec-menu="${menu.secMenu}"></td>
        <td style="width: 12.5%; text-align: center;"><input type="checkbox" class="permiso-checkbox" value="Actualizar" ${menu.actualizar ? 'checked' : ''} data-sec-menu="${menu.secMenu}"></td>
        <td style="width: 12.5%; text-align: center;"><input type="checkbox" class="permiso-checkbox" value="Eliminar" ${menu.eliminar ? 'checked' : ''} data-sec-menu="${menu.secMenu}"></td>
        ` : `
        <td style="width: 12.5%;"></td>
        <td style="width: 12.5%;"></td>
        <td style="width: 12.5%;"></td>
        <td style="width: 12.5%;"></td>
        `;
    html += crudPermisosHtml;

    html += '</tr>';

    // Submenús (se renderizan como filas adicionales, no anidadas en <ul>) - esto se manejará con data-parent-menu
    const subMenus = menu.subMenus && menu.subMenus.$values ? menu.subMenus.$values : menu.subMenus;
    if (subMenus && subMenus.length > 0) {
        subMenus.forEach(subMenu => {
            html += renderMenuItem(subMenu, level + 1);
        });
    }

    return html;
}

function collectPermissions(secRol, nombreRol) {
    const menus = [];

    // Recorrer todas las filas de la tabla
    $('#menuTreeContainer tbody tr').each(function () {
        const $row = $(this);
        const secMenu = parseInt($row.data('sec-menu'));
        const parentSecMenu = $row.data('parent-menu');

        if (!parentSecMenu) { 
            const menu = collectMenuItemPermissionsFromRow($row);
            if (menu) {
                menus.push(menu);
            }
        }
    });

    const menuMap = new Map();
    const rootMenus = [];

    $('#menuTreeContainer tbody tr').each(function () {
        const $row = $(this);
        const secMenu = parseInt($row.data('sec-menu'));
        const menuData = collectMenuItemPermissionsFromRow($row);
        if (menuData) {
            menuMap.set(secMenu, { ...menuData, SubMenus: [] });
        }
    });

    menuMap.forEach(menu => {
        const parentSecMenu = $('tr[data-sec-menu="' + menu.SecMenu + '"]').data('parent-menu');
        if (parentSecMenu && menuMap.has(parentSecMenu)) {
            menuMap.get(parentSecMenu).SubMenus.push(menu);
        } else {
            rootMenus.push(menu);
        }
    });

    return {
        SecRol: secRol,
        NombreRol: nombreRol,
        Menus: rootMenus
    };
}

function collectMenuItemPermissionsFromRow($row) {
    const secMenu = parseInt($row.data('sec-menu'));
    const $menuCheckbox = $row.find('.menu-checkbox');
    const verMenu = $menuCheckbox.prop('checked');

    const crear = $row.find('.permiso-checkbox[value="Crear"]').prop('checked');
    const leer = $row.find('.permiso-checkbox[value="Leer"]').prop('checked');
    const actualizar = $row.find('.permiso-checkbox[value="Actualizar"]').prop('checked');
    const eliminar = $row.find('.permiso-checkbox[value="Eliminar"]').prop('checked');

    return {
        SecMenu: secMenu,
        VerMenu: verMenu,
        Crear: crear,
        Leer: leer,
        Actualizar: actualizar,
        Eliminar: eliminar,
        SubMenus: []
    };
}