#include "Mesh.h"
#include <vector>
#include <string>
#define TINYOBJLOADER_IMPLEMENTATION
#include <tiny_obj_loader.h>
#include <exception>
#include "GraphicsEngine.h"
#include <comdef.h>

Mesh::Mesh(const wchar_t* fullPath) : Resource(fullPath)
{
	tinyobj::attrib_t attribs;
	std::vector<tinyobj::shape_t> shapes;
	std::vector<tinyobj::material_t> materials;
	std::string warn;
	std::string err;

	_bstr_t b(fullPath);
	const char* inputFile = b;

	bool result = tinyobj::LoadObj(&attribs, &shapes, &materials, &warn, &err, inputFile);

	if (!err.empty() || !result) throw std::exception("Mesh load had errors");

	std::vector<MeshPos0Tex0Normal0> listVertices;
	std::vector<unsigned int> listIndices;

	for (size_t s = 0; s < shapes.size(); s++)
	{
		size_t indexOffset = 0;
		listVertices.reserve(shapes[s].mesh.indices.size());
		listIndices.reserve(shapes[s].mesh.indices.size());

		for (size_t f = 0; f < shapes[s].mesh.num_face_vertices.size(); f++)
		{
			unsigned char numFaceVerts = shapes[s].mesh.num_face_vertices[f];

			for (unsigned char v = 0; v < numFaceVerts; v++)
			{
				tinyobj::index_t index = shapes[s].mesh.indices[indexOffset + v];
				tinyobj::real_t vx = attribs.vertices[(size_t)(index.vertex_index) * 3 + 0];
				tinyobj::real_t vy = attribs.vertices[(size_t)(index.vertex_index) * 3 + 1];
				tinyobj::real_t vz = attribs.vertices[(size_t)(index.vertex_index) * 3 + 2];

				tinyobj::real_t tx = attribs.texcoords[(size_t)(index.texcoord_index) * 2 + 0];
				tinyobj::real_t ty = attribs.texcoords[(size_t)(index.texcoord_index) * 2 + 1];

				tinyobj::real_t nx = attribs.normals[(size_t)(index.normal_index) * 3 + 0];
				tinyobj::real_t ny = attribs.normals[(size_t)(index.normal_index) * 3 + 1];
				tinyobj::real_t nz = attribs.normals[(size_t)(index.normal_index) * 3 + 2];

				listVertices.push_back(MeshPos0Tex0Normal0(Vector3(vx, vy, vz), Vector2(tx, ty), Vector3(nx, ny, nz)));
				listIndices.push_back((unsigned int)indexOffset + v);
			}

			indexOffset += numFaceVerts;
		}
	}

	m_VertexBuffer = GraphicsEngine::get()->getRenderSystem()->createVertexBuffer(&listVertices[0], (UINT)sizeof(MeshPos0Tex0Normal0), (UINT)listVertices.size());
	m_IndexBuffer = GraphicsEngine::get()->getRenderSystem()->createIndexBuffer(&listIndices[0], (UINT)listIndices.size());
}

Mesh::~Mesh()
{
	delete m_VertexBuffer;
	delete m_IndexBuffer;
}