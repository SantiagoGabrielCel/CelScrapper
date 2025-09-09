// API Configuration
const API_BASE = "https://celscrapper-api.onrender.com/api/Propiedades"

// Main search function
async function buscarPropiedades() {
  const loadingEl = document.getElementById("loading")
  const errorEl = document.getElementById("error")
  const tableEl = document.getElementById("propertiesTable")
  const statsEl = document.getElementById("stats")
  const searchBtn = document.querySelector(".search-button")

  try {
    // Show loading state with animations
    showLoading()
    hideError()
    hideResults()
    searchBtn.disabled = true
    searchBtn.innerHTML = "⏳ Procesando..."

    // Get form values
    const searchParams = getSearchParameters()

    // Build API URL
    const url = buildApiUrl(searchParams)
    console.log("🔍 Fetching from:", url)

    // Make API call with timeout
    const response = await fetchWithTimeout(url, 30000)

    if (!response.ok) {
      throw new Error(`Error HTTP: ${response.status} - ${response.statusText}`)
    }

    const properties = await response.json()
    console.log("✅ Properties received:", properties.length)

    if (Array.isArray(properties) && properties.length > 0) {
      displayProperties(properties)
      showStats(properties)

      // Add success animation
      setTimeout(() => {
        tableEl.classList.add("fade-in")
        statsEl.classList.add("slide-up")
      }, 100)
    } else {
      throw new Error("No se encontraron propiedades con los criterios seleccionados")
    }
  } catch (error) {
    console.error("❌ Error:", error)
    showError(`Error al cargar propiedades: ${error.message}`)
  } finally {
    hideLoading()
    searchBtn.disabled = false
    searchBtn.innerHTML = "🚀 Iniciar Búsqueda"
  }
}

// Helper function to get search parameters
function getSearchParameters() {
  return {
    tipo: document.getElementById("tipo").value,
    operacion: document.getElementById("operacion").value,
    zona: document.getElementById("zona").value,
    ambientes: document.getElementById("ambientes").value,
    paginas: document.getElementById("paginas").value,
    incluirScore: document.getElementById("incluirScore").checked,
  }
}

document.addEventListener("DOMContentLoaded", () => {
  const logo = document.getElementById("yesmanLogo")

  if (logo) {
    logo.addEventListener("click", () => {
      // Re-ejecutar la animación al hacer click
      logo.style.animation = "none"
      logo.offsetHeight // Trigger reflow
      logo.style.animation = "logoDropdown 0.8s ease-out"
    })
  }
})


// Helper function to build API URL
function buildApiUrl(params) {
  const queryParams = new URLSearchParams({
    tipo: params.tipo,
    operacion: params.operacion,
    zona: params.zona,
    ambientes: params.ambientes,
    maxPaginas: params.paginas,
    incluirScore: params.incluirScore,
  })

  return `${API_BASE}?${queryParams.toString()}`
}

// Fetch with timeout
async function fetchWithTimeout(url, timeout = 30000) {
  const controller = new AbortController()
  const timeoutId = setTimeout(() => controller.abort(), timeout)

  try {
    const response = await fetch(url, {
      signal: controller.signal,
      headers: {
        Accept: "application/json",
        "Content-Type": "application/json",
      },
    })
    clearTimeout(timeoutId)
    return response
  } catch (error) {
    clearTimeout(timeoutId)
    if (error.name === "AbortError") {
      throw new Error("La solicitud tardó demasiado tiempo. Intenta con menos páginas.")
    }
    throw error
  }
}

// Display properties in table
function displayProperties(properties) {
  const tbody = document.getElementById("propertiesBody")
  const table = document.getElementById("propertiesTable")

  tbody.innerHTML = ""

  properties.forEach((property, index) => {
    const row = document.createElement("tr")
    row.style.animationDelay = `${index * 0.05}s`
    row.classList.add("fade-in")

    row.innerHTML = `
            <td class="property-title">${escapeHtml(property.titulo || "Sin título")}</td>
            <td class="property-price">${formatPrice(property.precio, property.precioDecimal)}</td>
            <td class="property-address">${escapeHtml(property.direccion || "Sin dirección")}</td>
            <td class="property-description">${escapeHtml(truncateText(property.descripcion || "Sin descripción", 150))}</td>
            <td>
                ${
                  property.url
                    ? `<a href="${escapeHtml(property.url)}" target="_blank" class="property-link">
                        👁️ Ver Propiedad
                    </a>`
                    : '<span style="color: var(--muted-foreground);">Sin enlace</span>'
                }
            </td>
        `

    tbody.appendChild(row)
  })

  table.classList.add("show")
}

// Show statistics
function showStats(properties) {
  const statsEl = document.getElementById("stats")
  const statsText = document.getElementById("statsText")

  const count = properties.length
  const withPrices = properties.filter((p) => p.precioDecimal && p.precioDecimal > 0)
  const avgPrice =
    withPrices.length > 0 ? Math.round(withPrices.reduce((sum, p) => sum + p.precioDecimal, 0) / withPrices.length) : 0

  let statsMessage = `✅ ${count} propiedades encontradas`
  if (avgPrice > 0) {
    statsMessage += ` • 💰 Precio promedio: $${avgPrice.toLocaleString("es-AR")}`
  }

  // Add processing time if available
  const processingTime = Date.now() - window.searchStartTime
  if (window.searchStartTime) {
    statsMessage += ` • ⏱️ Procesado en ${(processingTime / 1000).toFixed(1)}s`
  }

  statsText.textContent = statsMessage
  statsEl.classList.add("show")
}

// Utility functions
function showLoading() {
  window.searchStartTime = Date.now()
  document.getElementById("loading").classList.add("show")
}

function hideLoading() {
  document.getElementById("loading").classList.remove("show")
}

function showError(message) {
  const errorEl = document.getElementById("error")
  errorEl.textContent = message
  errorEl.classList.add("show")
}

function hideError() {
  document.getElementById("error").classList.remove("show")
}

function hideResults() {
  document.getElementById("propertiesTable").classList.remove("show")
  document.getElementById("stats").classList.remove("show")
}

function formatPrice(priceText, priceDecimal) {
  if (priceDecimal && priceDecimal > 0) {
    return `$${priceDecimal.toLocaleString("es-AR")}`
  }
  return priceText || "Consultar"
}

function truncateText(text, maxLength) {
  if (!text) return "Sin descripción"
  return text.length > maxLength ? text.substring(0, maxLength) + "..." : text
}

function escapeHtml(text) {
  const div = document.createElement("div")
  div.textContent = text
  return div.innerHTML
}

// Initialize when page loads
document.addEventListener("DOMContentLoaded", () => {
  console.log("🚀 CelTech Property Scraper initialized")

  // Add keyboard shortcuts
  document.addEventListener("keydown", (e) => {
    if (e.ctrlKey && e.key === "Enter") {
      buscarPropiedades()
    }
  })

  // Add form validation
  const paginasInput = document.getElementById("paginas")
  paginasInput.addEventListener("input", function () {
    const value = Number.parseInt(this.value)
    if (value < 1) this.value = 1
    if (value > 10) this.value = 10
  })

  console.log("✅ Event listeners configured")
})

// Add some helpful console messages
console.log(`
🏢 CelTech Property Scraper
========================
Shortcuts:
• Ctrl + Enter: Iniciar búsqueda
• F12: Abrir herramientas de desarrollador

API Endpoint: ${API_BASE}
`)
